using FuncScript;
using FuncScript.Core;
using FuncScript.Error;
using FuncScript.Model;

namespace FsStudio.Server.FileSystem.Exec;

public class ExpressionNodeInfo
{
    public string Name { get; set; }
    public ExpressionType ExpressionType { get; set; }
    public int ChildrenCount { get; set; }
}

public class ExpressionNodeInfoWithExpression : ExpressionNodeInfo
{
    public string? Expression { get; set; }
    public string? CachedValue { get; set; }
    public bool IsCached { get; set; }
}

public enum ExpressionType
{
    ClearText,
    FuncScript,
    FuncScriptTextTemplate,
    FsStudioParentNode
}

public class ExecutionNode : KeyValueCollection
{
    private string _nameLower;
    private string _name;
    private object?_cache = null;
    private bool _cached = false;
    public ExpressionType ExpressionType { get; set; } = ExpressionType.FuncScript;
    public string? Expression { get; set; }
    public IList<ExecutionNode> Children { get; set; } = new List<ExecutionNode>();

    private KeyValueCollection _prentNode = null;
    public KeyValueCollection ParentContext => _prentNode;
    public object? GetCache() => _cache;
    public bool IsCached() => _cached;
    public void ClearCache()
    {
        _cache = null;
        _cached = false;
        foreach (var node in this.Children)
        {
            node.ClearCache();
        }
    }

    public object Get(string name)
    {
        var ch = Children.FirstOrDefault(c => c._nameLower.Equals(name));
        if (ch == null)
            return _prentNode.Get(name);
        return ch.Evaluate(this);
    }

    public void SetParent(KeyValueCollection parent)
    {
        this._prentNode = parent;
        foreach (var ch in Children)
        {
            ch.SetParent(this);
        }
    }

    public string NameLower => _nameLower;

    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            _nameLower = value.ToLower();
        }
    }

    public object Evaluate(KeyValueCollection provider)
    {
        if (Children.Count > 0)
        {
            return this;
        }
        if (_cached)
            return _cache;

        switch (ExpressionType)
        {
            case ExpressionType.ClearText:
                _cache= this.Expression;
                break;
            case ExpressionType.FuncScript:
                _cache=FuncScript.FuncScript.Evaluate(provider, Expression);
                break;
            case ExpressionType.FuncScriptTextTemplate:
                var serrors = new List<FuncScriptParser.SyntaxErrorData>();

                var result =
                    FuncScriptParser.ParseFsTemplate(
                        new FuncScriptParser.ParseContext(provider, this.Expression, serrors));
                if (result == null)
                    throw new SyntaxError(this.Expression, serrors);
                _cache= result.Block.Evaluate();
                break;
            default:
                throw new InvalidOperationException("Unsupported expression type");
        }

        _cached = true;
        return _cache;
    }


    public bool IsDefined(string key)
    {
        return Children.Any(c => c._nameLower == key);
    }

    public IList<string> GetAllKeys()
    {
        return this.Children.Select(c => c._name).ToList();
    }
}
