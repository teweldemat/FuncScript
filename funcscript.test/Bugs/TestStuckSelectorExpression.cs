using System.Collections.Generic;
using FuncScript.Core;
using FuncScript.Model;
using NUnit.Framework;

namespace FuncScript.Test.Bugs;

public class TestStuckSelectorExpression
{
    //[1,2,3] map ((x)=>5) filter (y)=>y<3
    [Test]
    public void TestStuckSelectorExpressionBug()
    {
        const string expStr = "{x:file('');p:f'{x}';}{p}";
        var errors = new List<FuncScriptParser.SyntaxErrorData>();
        var context = new FuncScriptParser.ParseContext(new DefaultFsDataProvider(), expStr, errors);
        var (exp, _, _) = FuncScriptParser.Parse(context);
        Assert.NotNull(exp);
        exp.SetReferenceProvider(context.ReferenceProvider);
        var res = exp.Evaluate();
        Assert.That(res, Is.AssignableTo<KeyValueCollection>());
        var kv = (KeyValueCollection)res;
        Assert.That(kv.Get("p"), Is.TypeOf<FsError>());
    }
}
