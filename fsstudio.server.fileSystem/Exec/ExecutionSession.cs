using System.Text;
using System.Text.RegularExpressions;
using FsStudio.Server.FileSystem.Exec.Funcs;
using FuncScript;
using FuncScript.Core;
using FuncScript.Funcs.Text;
using FuncScript.Host;
using FuncScript.Model;

namespace FsStudio.Server.FileSystem.Exec;

public class ExecutionSession : KeyValueCollection
{
    List<ExecutionNode> _nodes;
    private KeyValueCollection _context;
    readonly string fileName;
    public Guid SessionId { get; private set; } = Guid.NewGuid();
    public KeyValueCollection ParentContext => _context;

    private RemoteLogger? _logger=null;
    private Task<object>? _evaluationTask;
    private Exception? _evaluationException;
    private object? _evaluationResult;

    public bool IsEvaluationInProgress => _evaluationTask != null && !_evaluationTask.IsCompleted;
    public bool IsEvaluationCompleted => _evaluationTask != null && _evaluationTask.IsCompleted;
    public object? LastEvaluationResult => _evaluationResult;
    public Exception? LastEvaluationException => _evaluationException;

    public ExecutionSession(string fileName, RemoteLogger logger)
    {
        this._logger = logger;
        this.fileName = fileName;
        var json = System.IO.File.ReadAllText(fileName);
        InitFromNodes(System.Text.Json.JsonSerializer.Deserialize<List<ExecutionNode>>(json) ?? []);
    }

    void InitFromNodes(IEnumerable<ExecutionNode> nodes)
    {
        _nodes = nodes.ToList();
        foreach (var n in _nodes)
            n.SetParent(this);
        this._context = new DefaultFsDataProvider(
            new[]
            {
                KeyValuePair.Create<string, object>("md", new MarkDownFunction(this._logger, this.SessionId.ToString()))
            }
        );
    }

    public ExecutionSession(IEnumerable<ExecutionNode> nodes, RemoteLogger logger)
    {
        this._logger = logger;
        InitFromNodes(nodes);
    }

    void UpdateFile()
    {
        System.IO.File.WriteAllText(fileName, System.Text.Json.JsonSerializer.Serialize(_nodes));
    }

    private ExecutionNode? FindNodeByPath(string nodePath)
    {
        var segments = nodePath.Split('.');
        ExecutionNode? currentNode = null;

        foreach (var segment in segments)
        {
            currentNode = (currentNode == null ? _nodes : currentNode.Children)
                .FirstOrDefault(n => n.NameLower == segment.ToLower());

            if (currentNode == null)
                break;
        }

        return currentNode;
    }

    public void CreateNode(string? parentNodePath, string name, string expression, ExpressionType expressionType)
    {
        if (!ValidName(name))
            throw new InvalidOperationException($"{name} is invalid");

        var parentNode = parentNodePath == null ? null : FindNodeByPath(parentNodePath);
        var nodes = parentNodePath == null ? _nodes : parentNode?.Children;
        if (nodes == null)
            throw new InvalidOperationException($"Path {parentNodePath} not found");
        var nameLower = name.ToLower();
        if (nodes.Any(n => n.NameLower == nameLower))
            throw new InvalidOperationException($"Name: {name} already used");
        var n = new ExecutionNode
        {
            Name = name,
            Expression = expression,
            ExpressionType = expressionType
        };
        if (parentNode != null)
        {
            if (!string.IsNullOrEmpty(parentNode.Expression))
            {
                var backupChild = new ExecutionNode
                {
                    Name = $"{parentNode.Name}_backup",
                    Expression = parentNode.Expression,
                    ExpressionType = parentNode.ExpressionType
                };
                parentNode.Children.Add(backupChild);
            }

            parentNode.ExpressionType = ExpressionType.FsStudioParentNode;
            parentNode.Expression = null;
        }

        n.SetParent(parentNode == null ? this : parentNode);
        nodes.Add(n);
        UpdateFile();
    }

    public void RemoveNode(string nodePath)
    {
        var segments = nodePath.Split('.');
        var parentNodePath = string.Join(".", segments.Take(segments.Length - 1));
        ExecutionNode? parentNode = null;
        var nodes = segments.Length == 1 ? _nodes : (parentNode = FindNodeByPath(parentNodePath))?.Children;
        if (nodes == null)
            throw new InvalidOperationException($"Path {parentNodePath} not found");

        var nodeName = segments.Last().ToLower();
        var index = nodes.Select(n => n.NameLower).ToList().IndexOf(nodeName);
        if (index != -1)
        {
            nodes.RemoveAt(index);
            if (nodes.Count == 0 && parentNode != null)
            {
                parentNode.ExpressionType = ExpressionType.FuncScript;
            }
        }

        UpdateFile();
    }

    public ExecutionNode? GetParentPath(string nodePath)
    {
        var segments = nodePath.Split('.');
        if (segments.Length == 1)
            return null;
        var parentNodePath = string.Join(".", segments.Take(segments.Length - 1));
        return FindNodeByPath(parentNodePath);
    }

    public static bool ValidName(string name)
    {
        Regex regex = new Regex(@"^[a-zA-Z_$][a-zA-Z0-9_$]*$");
        return regex.IsMatch(name);
    }

    public void RenameNode(string nodePath, string newName)
    {
        if (!ValidName(newName))
            throw new InvalidOperationException($"{newName} is invalid");
        var node = FindNodeByPath(nodePath);
        if (node == null)
            throw new Exception("Node not found.");
        var parent = GetParentPath(nodePath);
        var _namelower = newName.ToLower();
        if (parent != null)
        {
            if (parent.Children.Any(ch => ch.NameLower == _namelower))
                throw new InvalidOperationException($"{newName} already exists");
        }

        node.Name = newName;
        UpdateFile();
    }

    public void ChangeExpressionType(string nodePath, ExpressionType expType)
    {
        var node = FindNodeByPath(nodePath);
        if (node == null)
            throw new Exception("Node not found.");
        node.ExpressionType = expType;
        UpdateFile();
    }

    public void UpdateExpression(string nodePath, string expression,bool updateFile)
    {
        var node = FindNodeByPath(nodePath);
        if (node == null)
            throw new Exception("Node not found.");
        if (node.Children.Count > 0)
            throw new Exception("Expression can't be set to a parent node");
        node.Expression = expression;
        if(updateFile)
            UpdateFile();
    }

    public List<ExpressionNodeInfo> GetChildNodeList(string? nodePath)
    {
        var nodes = nodePath == null ? _nodes : FindNodeByPath(nodePath)?.Children;
        if (nodes == null)
            throw new InvalidOperationException($"Path {nodePath} not found");

        return nodes.Select(c => new ExpressionNodeInfo
        {
            Name = c.Name,
            ExpressionType = c.ExpressionType,
            ChildrenCount = c.Children.Count
        }).ToList();
    }

    public ExpressionNodeInfoWithExpression? GetExpression(string nodePath)
    {
        var node = FindNodeByPath(nodePath);
        if (node == null)
            return null;
        var c = node.GetCache();

        string? json = null;
        try
        {
            json = c == null ? null : FuncScript.Helpers.FormatToJson(c);
        }
        catch (Exception e)
        {
            json = e.Message;
        }

        return new ExpressionNodeInfoWithExpression
        {
            Name = node.Name,
            ExpressionType = node.ExpressionType,
            ChildrenCount = node.Children.Count,
            Expression = node.Expression,
            CachedValue = json,
            IsCached = node.IsCached()
        };
    }

    public object Get(string name)
    {
        var n = _nodes.FirstOrDefault(c => c.NameLower == name);
        if (n == null)
            return _context.Get(name);
        return n.Evaluate(this);
    }

    public bool IsDefined(string name)
    {
        var n = _nodes.FirstOrDefault(c => c.NameLower == name);
        if (n != null)
            return true;
        return _context.IsDefined(name);
    }

    public IList<string> GetAllKeys()
    {
        return _nodes.Select(x => x.Name).ToList();
    }

    void ClearCache()
    {
        foreach (var node in _nodes)
        {
            node.ClearCache();
        }
    }

    object EvaluateNodeInternal(string nodePath)
    {
        ClearCache();
        if(_logger!=null)
            FsLogger.SetDefaultLogger(new SessionManager.RemoteLoggerForFs(_logger, SessionId.ToString()));
        var segments = nodePath.Split('.');
        var parentNodePath = string.Join(".", segments.Take(segments.Length - 1));
        var provider = (segments.Length > 1) ? (KeyValueCollection)FindNodeByPath(parentNodePath)! : this;
        return provider.Get(segments.Last());
    }

    public Task<object> EvaluateNodeAsync(string nodePath)
    {
        if (IsEvaluationInProgress)
            throw new InvalidOperationException("Only one evaluation is supported at a time.");

        _evaluationException = null;
        _evaluationResult = null;

        _evaluationTask = Task.Run(() =>
        {
            try
            {
                var res = EvaluateNodeInternal(nodePath.ToLowerInvariant());
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        });

        _evaluationTask.ContinueWith(task =>
        {
            if (task.IsFaulted && task.Exception != null)
            {
                _evaluationException = task.Exception.GetBaseException();
                var msg = _evaluationException.Message + "\n" + _evaluationException.StackTrace;
                var ex = _evaluationException.InnerException;
                while (ex != null)
                {
                    msg += $"\n{ex.Message}\n{ex.StackTrace}";
                    ex = ex.InnerException;
                }

                if (_logger != null)
                {
                    _logger.SendObject("evaluation_error", new
                    {
                        sessionId = SessionId,
                        error = msg
                    });
                }
            }
            else
            {
                _evaluationResult = task.Result;
                var sb = new StringBuilder();
                FuncScript.Helpers.Format(sb, _evaluationResult,
                    asJsonLiteral: _evaluationResult is KeyValueCollection || _evaluationResult is FsList
                );
                if (_logger != null)
                {
                    _logger.SendObject("evaluation_success", new
                    {
                        sessionId = SessionId,
                        result = sb.ToString()
                    });
                }
            }
        }, TaskContinuationOptions.ExecuteSynchronously);

        return _evaluationTask;
    }

    public void MoveNode(string modelNodePath, string? modelNewParentPath)
    {
        var node = FindNodeByPath(modelNodePath);
        if (node == null)
            throw new InvalidOperationException($"Node {modelNodePath} not found");

        var segments = modelNodePath.Split('.');
        var currentParentPath = string.Join(".", segments.Take(segments.Length - 1));
        var currentParent = segments.Length == 1 ? null : FindNodeByPath(currentParentPath);
        var currentChildrenList = currentParent == null ? _nodes : currentParent.Children;
        currentChildrenList.Remove(node);

        var newParent = modelNewParentPath == null ? null : FindNodeByPath(modelNewParentPath);
        if (modelNewParentPath != null && newParent == null)
            throw new InvalidOperationException($"New parent {modelNewParentPath} not found");
        var newChildrenList = newParent == null ? _nodes : newParent.Children;
        if (newChildrenList.Any(n => n.NameLower == node.NameLower))
            throw new InvalidOperationException(
                $"Node named {node.Name} already exists under {modelNewParentPath ?? "root"}");

        if (newParent != null && !string.IsNullOrEmpty(newParent.Expression))
        {
            var backupChild = new ExecutionNode
            {
                Name = $"{newParent.Name}_backup",
                Expression = newParent.Expression,
                ExpressionType = newParent.ExpressionType
            };
            newParent.Children.Add(backupChild);
            newParent.ExpressionType = ExpressionType.FsStudioParentNode;
            newParent.Expression = null;
        }

        node.SetParent(newParent == null ? this : newParent);
        newChildrenList.Add(node);
        UpdateFile();
    }
}