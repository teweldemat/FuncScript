using System;
using System.Collections.Generic;
using funcscript.core;
using funcscript.host;
using funcscript.model;
using NUnit.Framework;

namespace funcscript.test;

public class TestEconomics
{
    [Test]
    public void OnlyOneCallExpectedTest()
    {
        
        var exp = @"{
  f:(c)=>c;
  return [f(log(""x"",'start'))]
}";
        var logger = new StringLogger();
        FsLogger.SetDefaultLogger(logger);
        var res=FuncScript.Evaluate(new DefaultFsDataProvider(), exp);
        var str = FuncScript.FormatToJson(res);
        Assert.That(res is FsList);
        var l = (FsList)res;
        Assert.That(l.Length,Is.EqualTo(1));
        Assert.That(l[0],Is.EqualTo("x"));
        Assert.That(logger.GetLogContent().Trim(),Is.EqualTo("start"));
    }
    
    [Test]
    public void OnlyOneCallExpectedTest2()
    {

        var exp = @"([log('x','x')] filter (a)=>a!=2) map (x)=>1=1";
        var logger = new StringLogger();
        FsLogger.SetDefaultLogger(logger);
        var res=FuncScript.Evaluate(new DefaultFsDataProvider(), exp);
        var str = FuncScript.FormatToJson(res);
        Assert.That(logger.GetLogContent().Trim(),Is.EqualTo("x"));
    }
}