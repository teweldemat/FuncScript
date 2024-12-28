using System.Text.RegularExpressions;
using funcscript;
using funcscript.core;
using funcscript.model;

namespace fsstudio.server.fileSystem.exec;

public class ExecutionSession : KeyValueCollection
{
    List<ExecutionNode> _nodes;
    private KeyValueCollection _context;
    readonly string fileName;
    public Guid SessionId { get; private set; } = Guid.NewGuid();
    public KeyValueCollection ParentContext => _context;

   


    private RemoteLogger logger;
    public ExecutionSession(string fileName,RemoteLogger logger)
    {
        this.logger = logger;
        this.fileName = fileName;
        var json=System.IO.File.ReadAllText(fileName);
        InitFromNodes(System.Text.Json.JsonSerializer.Deserialize<List<ExecutionNode>>(json)??[]);
    }

    void InitFromNodes(IEnumerable<ExecutionNode> nodes)
    {
        _nodes =nodes.ToList() ;
        foreach(var n in _nodes)
            n.SetParent(this);
        this._context = new DefaultFsDataProvider();
    }
    public ExecutionSession(IEnumerable<ExecutionNode> nodes,RemoteLogger logger)
    {
        this.logger = logger;
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

    public void CreateNode(string? parentNodePath, string name, string expression,ExpressionType expressionType)
    {
        if (!ValidName(name))
            throw new InvalidOperationException($"{name} is invalid");

        var parentNode =parentNodePath==null?null:  this.FindNodeByPath(parentNodePath);
        var nodes = parentNodePath == null ? this._nodes : parentNode?.Children;
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
            if(!string.IsNullOrEmpty(parentNode.Expression))
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
        n.SetParent(parentNode==null?this:parentNode);
        nodes.Add(n);
        UpdateFile();
    }

   
    public void RemoveNode(string nodePath)
    {
        var segments = nodePath.Split('.');
        var parentNodePath = string.Join(".", segments.Take(segments.Length - 1));
        ExecutionNode ? parentNode=null;
        var nodes=segments.Length==1?this._nodes:(parentNode=FindNodeByPath(parentNodePath))?.Children;
        if (nodes == null)
            throw new InvalidOperationException($"Path {parentNodePath} not found");

        var nodeName = segments.Last().ToLower();
        var index = nodes.Select(n=>n.NameLower).ToList().IndexOf(nodeName);
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
        // Regular expression for a valid JavaScript identifier
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
    public void UpdateExpression(string nodePath, string expression)
    {
        var node = FindNodeByPath(nodePath);
        if (node == null)
            throw new Exception("Node not found.");
        if (node.Children.Count > 0)
            throw new Exception("Expression can't be set to a parent node");
        node.Expression = expression;
        UpdateFile();
    }

    public List<ExpressionNodeInfo> GetChildNodeList(string? nodePath)
    {
        var nodes = nodePath == null ?this._nodes: this.FindNodeByPath(nodePath)?.Children;
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
        var node= FindNodeByPath(nodePath);
        if (node == null)
            return null;
        return new ExpressionNodeInfoWithExpression
        {
            Name = node.Name,
            ExpressionType = node.ExpressionType,
            ChildrenCount = node.Children.Count,
            Expression = node.Expression
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
        return this._nodes.Select(x => x.Name).ToList();
    }


    public object EvaluateNode(string nodePath)
    {
        var segments = nodePath.Split('.');
        var parentNodePath = string.Join(".", segments.Take(segments.Length - 1));
        var provider = (segments.Length > 1) ? (KeyValueCollection)FindNodeByPath(parentNodePath)! : this;
        var res=provider.Get(segments.Last());
        return res;
    }

}