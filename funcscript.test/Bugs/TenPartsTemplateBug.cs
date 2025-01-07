using System.Collections.Generic;
using FuncScript.Core;
using NUnit.Framework;

namespace FuncScript.Test.Bugs;

public class TenPartsTemplateBug
{
    [Test]
    public void TestStringLiteral()
    {
        const string expStr ="f\"1{'2'}3{'4'}5{'6'}7{'8'}9{'A'}B\"";
        var errors = new List<FuncScriptParser.SyntaxErrorData>();
        var context = new FuncScriptParser.ParseContext(new DefaultFsDataProvider(), expStr, errors);
        var (exp, node, _) = FuncScriptParser.Parse(context);
        var res = exp.Evaluate();
        Assert.That(res, Is.EqualTo("123456789AB"));
    }

}
