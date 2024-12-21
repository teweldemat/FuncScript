using fsstudio.server.fileSystem.exec;

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
}