using fsstudio.server.fileSystem.exec;
using funcscript;
using funcscript.host;
using funcscript.model;

namespace fsstudio.test;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestRelativeRef()
    {
        var nodes = new[]
        {
            new ExecutionNode
            {
                Name = "x",
                Expression = null,
                ExpressionType = ExpressionType.FsStudioParentNode,
                Children = new ExecutionNode[]
                {
                    new ExecutionNode()
                    {
                        Name = "y",
                        Expression = "a.b",
                        ExpressionType = ExpressionType.FuncScript,
                    },
                    new ExecutionNode()
                    {
                        Name = "a",
                        Expression = null,
                        ExpressionType = ExpressionType.FsStudioParentNode,
                        Children = new ExecutionNode[]
                        {
                            new ExecutionNode()
                            {
                                Name = "b",
                                Expression = "11",
                                ExpressionType = ExpressionType.FuncScript,
                            }
                        }
                    }
                }
            }
        };
        var session = new ExecutionSession(nodes, null);
        var res = session.EvaluateNode("x.y");
        Assert.That(res is int);
        Assert.That((int)res, Is.EqualTo(11));
    }

    [Test]
    public void NodeVariable()
    {
        var nodes = new[]
        {
            new ExecutionNode()
            {
                Name = "y",
                Expression = "6",
                ExpressionType = ExpressionType.FuncScript,
            },
            new ExecutionNode()
            {
                Name = "x",
                Expression = "y+5",
                ExpressionType = ExpressionType.FuncScript,
            }
        };
        var session = new ExecutionSession(nodes, null);
        var res = session.EvaluateNode("x");
        Assert.That(res is int);
        Assert.That((int)res, Is.EqualTo(11));
    }
    
    [Test]
    public void TestLogger()
    {
        var logger = new StringLogger();
        FsLogger.SetDefaultLogger(logger);
        var exp = @"{
  f:(c)=>c;
  return [f(log(""x"",'start'))]
}";
        var nodes = new[]
        {
            new ExecutionNode
            {
                Name = "x",
                Expression = exp,
                ExpressionType = ExpressionType.FuncScript,
                Children = new ExecutionNode[0]
            }                   
        };
        var session = new ExecutionSession(nodes, null);
        var res = session.EvaluateNode("x");
        Assert.That(res is FsList);
        var l = (FsList)res;
        Assert.That(l.Length,Is.EqualTo(1));
        Assert.That(l[0],Is.EqualTo("x"));
        Assert.That(logger.GetLogContent().Trim(),Is.EqualTo("start"));

    }
    
    [Test]
    public void TestCaching()
    {
        var logger = new StringLogger();
        FsLogger.SetDefaultLogger(logger);
        var exp1 = "'x' log 'a'";
        var exp2 = @"{
  r:l+'y';
  m:l+'z'
}";
        var nodes = new[]
        {
            new ExecutionNode
            {
                Name = "l",
                Expression = exp1,
                ExpressionType = ExpressionType.FuncScript,
                Children = new ExecutionNode[0]
            },
            new ExecutionNode
            {
                Name = "dl",
                Expression = exp2,
                ExpressionType = ExpressionType.FuncScript,
                Children = new ExecutionNode[0]
            } 
        };
        var session = new ExecutionSession(nodes, null);
        var res = session.EvaluateNode("dl");
        FuncScript.FormatToJson(res);
        Assert.That(logger.GetLogContent().Trim(),Is.EqualTo("a"));

    }
    [Test]
    public void TestCacheClearing()
    {
        var logger = new StringLogger();
        FsLogger.SetDefaultLogger(logger);
        var exp1 = "5 log 'a'";
        var exp2 = @"{
  r:l+'y';
  m:l+'z'
}";
        var nodes = new[]
        {
            new ExecutionNode
            {
                Name = "l",
                Expression = exp1,
                ExpressionType = ExpressionType.FuncScript,
                Children = Array.Empty<ExecutionNode>()
            },
            new ExecutionNode
            {
                Name = "dl",
                Expression = exp2,
                ExpressionType = ExpressionType.FuncScript,
                Children = Array.Empty<ExecutionNode>()
            } 
        };
        var session = new ExecutionSession(nodes, null);
        var res = session.EvaluateNode("dl");
        var json=FuncScript.FormatToJson(res);
        Assert.That(logger.GetLogContent().Trim(),Is.EqualTo("a"));
        Assert.That(FuncScript.FormatToJson(FuncScript.NormalizeDataType(new {r="5y",m="5z"})),Is.EqualTo(json));

        logger.Clear();
        nodes[0].Expression = "2 log 'b'";
        res = session.EvaluateNode("dl");
        json=FuncScript.FormatToJson(res);
        Assert.That(logger.GetLogContent().Trim(),Is.EqualTo("b"));
        
        Assert.That(FuncScript.FormatToJson(FuncScript.NormalizeDataType(new {r="2y",m="2z"})),Is.EqualTo(json));

    }
}