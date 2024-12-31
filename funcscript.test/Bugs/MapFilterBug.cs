using System.Collections.Generic;
using funcscript.core;
using NUnit.Framework;

namespace funcscript.test.Bugs;

public class MapFilterBug
{
    //[1,2,3] map ((x)=>5) filter (y)=>y<3
    [Test]
    public void TestMapFilterBug()
    {
        const string expStr ="[1,2,3] map ((x)=>5) filter (y)=>y<3";
        var errors = new List<FuncScriptParser.SyntaxErrorData>();
        var context = new FuncScriptParser.ParseContext(new DefaultFsDataProvider(), expStr, errors);
        var (exp, node, _) = FuncScriptParser.Parse(context);
        Assert.NotNull(node);
        Assert.That(node.NodeType,Is.EqualTo(FuncScriptParser.ParseNodeType.InfixExpression));
        Assert.That(node.Children.Count,Is.EqualTo(3));
        Assert.That(node.Children[2].NodeType,Is.EqualTo(FuncScriptParser.ParseNodeType.InfixExpression));
        var res = exp.Evaluate();
        Assert.That(FuncScript.FormatToJson(res), Is.EqualTo("[]"));
    }
}