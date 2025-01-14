using NUnit.Framework;

namespace FuncScript.Test.Curated;

public class TestCompoundComparision
{
    [Test]
    public void TestEmptyListEquals()
    {
        var expStr = "[]=[]";
        var p = new DefaultFsDataProvider();
        var ret = Helpers.Evaluate(p, expStr);
        Assert.AreEqual(true, Helpers.NormalizeDataType(ret));
    }
}