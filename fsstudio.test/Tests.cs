using fsstudio.server.fileSystem.exec;
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
}