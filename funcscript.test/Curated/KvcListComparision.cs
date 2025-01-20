using FuncScript.Model;
using NUnit.Framework;

namespace FuncScript.Test.Curated;

public class KvcListComparision
{
    [Test]
    public void TestKvcComparision1()
    {
        var exp = "{x:2}={x:2}";
        var res = Helpers.Evaluate(exp);
        Assert.That(res, Is.EqualTo(true));
    }
    [Test]
    public void TestKvcComparision1_not_equal()
    {
        var exp = "{xX:2}={Xx:3}";
        var res = Helpers.Evaluate(exp);
        Assert.That(res, Is.EqualTo(false));
    }
    
    [Test]
    public void TestKvcComparision1_object_kvc()
    {
        var exp = "{x:2}=a";
        var res = Helpers.EvaluateWithVars(exp,new
        {
            a=new {x=2}
        });
        Assert.That(res, Is.EqualTo(true));
    }
    
    [Test]
    public void TestKvcComparision2()
    {
        var exp = "{x:2,y:3}={x:2,y:3}";
        var res = Helpers.Evaluate(exp);
        Assert.That(res, Is.EqualTo(true));
    }

    [Test]
    public void TestKvcComparision3_NestedSame()
    {
        var exp = "{x:{y:2}} = {x:{y:2}}";
        var res = Helpers.Evaluate(exp);
        Assert.That(res, Is.EqualTo(true));
    }
    
    [Test]
    public void TestKvcComparision4_NestedDifferent()
    {
        var exp = "{x:{y:2}} = {x:{y:3}}";
        var res = Helpers.Evaluate(exp);
        Assert.That(res, Is.EqualTo(false));
    }
    
    [Test]
    public void TestKvcComparision5_MultipleNested()
    {
        var exp = "{a:{b:{c:1}}, x:2} = {a:{b:{c:1}}, x:2}";
        var res = Helpers.Evaluate(exp);
        Assert.That(res, Is.EqualTo(true));
    }

    [Test]
    public void TestKvcComparision6_MultipleNestedDifferentKey()
    {
        var exp = "{a:{b:{c:1}}, x:2} = {a:{b:{d:1}}, x:2}";
        var res = Helpers.Evaluate(exp);
        Assert.That(res, Is.EqualTo(false));
    }
    
    [Test]
    public void TestKvcComparision7_OrderIrrelevant()
    {
        var exp = "{x:2, y:3} = {y:3, x:2}";
        var res = Helpers.Evaluate(exp);
        Assert.That(res, Is.EqualTo(true));
    }

    [Test]
    public void TestKvcComparision8_PartialKeys()
    {
        var exp = "{x:2, y:3} = {x:2}";
        var res = Helpers.Evaluate(exp);
        Assert.That(res, Is.EqualTo(false));
    }
    
    [Test]
    public void TestKvcComparision9_EmptyStructures()
    {
        var exp = "{} = {}";
        var res = Helpers.Evaluate(exp);
        Assert.That(res, Is.EqualTo(true));
    }

    [Test]
    public void TestKvcComparision10_DeeperNestedEquality()
    {
        var exp = "{p:{q:{r:{s:99}}}} = {p:{q:{r:{s:99}}}}";
        var res = Helpers.Evaluate(exp);
        Assert.That(res, Is.EqualTo(true));
    }
}