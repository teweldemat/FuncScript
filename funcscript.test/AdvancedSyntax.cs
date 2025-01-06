using System;
using funcscript.model;
using NUnit.Framework;

namespace funcscript.test;

public class AdvancedSyntax
{
    [Test]
    public void NakedKeyValuePair()
    {
        var exp = "a:4,b:5";
        var res = FuncScript.Evaluate(exp);
        var expected = new ObjectKvc(new { a = 4, b = 5 });
        Assert.That(FuncScript.FormatToJson(res),Is.EqualTo(FuncScript.FormatToJson(expected)));
    }
    
    [Test]
    public void NakedKeyValuePairTrailingSpace()
    {
        var exp = "a:4,b:5 ";
        var res = FuncScript.Evaluate(exp);
        var expected = new ObjectKvc(new { a = 4, b = 5 });
        Assert.That(FuncScript.FormatToJson(res),Is.EqualTo(FuncScript.FormatToJson(expected)));
    }
    
    [Test]
    public void NakedWithImplicitReturn()
    {
        var exp = "a:4; a*3";
        var res = FuncScript.Evaluate(exp);
        var expected = 12;
        Assert.That(FuncScript.FormatToJson(res),Is.EqualTo(FuncScript.FormatToJson(expected)));
    }
    [Test]
    public void NakedWithImplicitReturn2()
    {
        var exp = "x:30; x;";
        var res = FuncScript.Evaluate(exp);
        var expected = 30;
        Assert.That(FuncScript.FormatToJson(res),Is.EqualTo(FuncScript.FormatToJson(expected)));
    }
    [Test]
    public void NakedWithExplicitReturn2()
    {
        var exp = "x:30; return x;";
        var res = FuncScript.Evaluate(exp);
        var expected = 30;
        Assert.That(FuncScript.FormatToJson(res),Is.EqualTo(FuncScript.FormatToJson(expected)));
    }
    [Test]
    [TestCase("!true",false)]
    [TestCase("!false",true)]
    [TestCase("!(1=2)",true)]
    public void NotOperator(string exp,object expected)
    {
        var res = FuncScript.Evaluate(exp);
        Assert.That(res,Is.EqualTo(expected));
    }
    [Test]
    [TestCase("-5",-5)]
    [TestCase("1--5",6)]
    [TestCase("1+-5",-4)]
    [TestCase("{x:-5;return -x}",5)]
    [TestCase("{x:-5;return 1--x}",-4)]
    [TestCase("{x:-5;return 1+-x}",6)]
    public void NegOperator(string exp,object expected)
    {
        var res = FuncScript.Evaluate(exp);
        Assert.That(res,Is.EqualTo(expected));
    }
    
    [Test]
    [TestCase("reduce([4,5,6],(x,s)=>s+x)",15)]
    [TestCase("reduce([4,5,6],(x,s)=>s+x,-2)",13)]
    [TestCase("[4,5,6] reduce (x,s)=>s+x ~ -2",13)]
    [TestCase("(series(0,4) reduce (x,s)=>s+x ~ 0)",6)]
    [TestCase("series(0,4) reduce (x,s)=>s+x ~ 0",6)]
    [TestCase("(series(1,3) map (a)=>a*a) reduce (x,s)=>s+x ~ 5",19)]
    [TestCase("x?![1,2,3] first(x)=>x*x",null)]
    [TestCase("{ b:x?! [1,2,3] map(x) => 5; return b}",null)]
    [TestCase("{x:9; b:x?! [1,2,3] map(x) => 5; return b[1]}",5)]
    public void GeneralInfix(string exp,object expected)
    {
        var res = FuncScript.Evaluate(exp);
        Assert.That(res,Is.EqualTo(expected));
    }
    
    [Test]
    [TestCase("1+2*4",9)]
    [TestCase("1+4/2",3)]
    public void Precidence(string exp,object expected)
    {
        var res = FuncScript.Evaluate(exp);
        Assert.That(res,Is.EqualTo(expected));
    }
    
    [Test]
    [TestCase("!null",FsError.ERROR_TYPE_MISMATCH)]
    public void ErrorResults(string exp,string type)
    {
        var res = FuncScript.Evaluate(exp);
        Assert.That(res, Is.TypeOf<FsError>());
        Assert.That(((FsError)res).ErrorType,Is.EqualTo(type));
    }
}