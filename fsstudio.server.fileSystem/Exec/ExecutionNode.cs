using funcscript;
using funcscript.core;
using funcscript.error;
using funcscript.model;

namespace fsstudio.server.fileSystem.exec;

public class ExpressionNodeInfo
{
    public string Name { get; set; }
    public ExpressionType ExpressionType { get; set; }
    public int ChildrenCount { get; set; }
}


public class ExpressionNodeInfoWithExpression:ExpressionNodeInfo
{
    public String? Expression { get; set; }
}

public enum ExpressionType
{
    ClearText,
    FuncScript,
    FuncScriptTextTemplate,
    FsStudioParentNode
}

public class ExecutionNode : KeyValueCollection, IFsDataProvider
{
    private string _nameLower;
    private string _name;
    private IFsDataProvider _prentNode = null;

    public void SetParent(IFsDataProvider parent)
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

    public ExpressionType ExpressionType { get; set; } = ExpressionType.FuncScript;
    public string? Expression { get; set; }
    public IList<ExecutionNode> Children { get; set; }= new List<ExecutionNode>();

    public object Evaluate(IFsDataProvider provider)
    {
        if (Children.Count > 0)
        {
            return this;
        }

        switch (ExpressionType)
        {
            case ExpressionType.ClearText:
                return this.Expression;
            case ExpressionType.FuncScript:
                return FuncScript.Evaluate(provider, Expression);
            case ExpressionType.FuncScriptTextTemplate:
                var serrors = new List<FuncScriptParser.SyntaxErrorData>();
                var exp = FuncScriptParser.ParseFsTemplate(provider, this.Expression, serrors);
                if (exp == null)
                    throw new SyntaxError(serrors);
                return exp.Evaluate(provider);
            default:
                throw new InvalidOperationException("Unsupported expression type");
        }
    }

    public override object Get(string key)
    {
        var ch = Children.FirstOrDefault(c => c._nameLower.Equals(key));
        if (ch == null)
            return null;
        return ch.Evaluate(this);
    }

    public override bool ContainsKey(string key)
    {
        return Children.Any(c => c._nameLower == key);
    }

    public override IList<KeyValuePair<string, object>> GetAll()
    {
        return this.Children.Select(c => KeyValuePair.Create(c._name, c.Evaluate(this))).ToList();
    }

    public object GetData(string name)
    {
        var ch = Children.FirstOrDefault(c => c._nameLower.Equals(name));
        if (ch == null)
            return _prentNode.GetData(name);
        return ch.Evaluate(this);
    }
}