
using FsStudio.Server.FileSystem.Exec;
using FuncScript;
using FuncScript.Host;
using FuncScript.Model;

namespace FsStudio.Server.FileSystem.Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task TestRelativeRef()
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
        var res = await session.EvaluateNodeAsync("x.y");
        Assert.That(res is int);
        Assert.That((int)res, Is.EqualTo(11));
    }

    [Test]
    public async Task  NodeVariable()
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
        var res = await session.EvaluateNodeAsync("x");
        Assert.That(res is int);
        Assert.That((int)res, Is.EqualTo(11));
    }
    
    [Test]
    public async Task  TestLogger()
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
        var res = await session.EvaluateNodeAsync("x");
        Assert.That(res is FsList);
        var l = (FsList)res;
        Assert.That(l.Length,Is.EqualTo(1));
        Assert.That(l[0],Is.EqualTo("x"));
        Assert.That(logger.GetLogContent().Trim(),Is.EqualTo("start"));

    }
    
    [Test]
    public async Task  TestLoggerLogBug()
    {
        var logger = new StringLogger();
        FsLogger.SetDefaultLogger(logger);
        var exp = @"{a:'ls' log 'running'}";
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
        var res = await session.EvaluateNodeAsync("x");
        var json = Helpers.FormatToJson(res);
        var n = logger.GetLogContent().Split("running").Length - 1;
        Assert.That(n,Is.EqualTo(1));
    }
    
    [Test]
    public async Task TestCaching()
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
        var res =await session.EvaluateNodeAsync("dl");
        FuncScript.Helpers.FormatToJson(res);
        Assert.That(logger.GetLogContent().Trim(),Is.EqualTo("a"));

    }
    [Test]
    public async Task TestCacheClearing()
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
        var res = await session.EvaluateNodeAsync("dl");
        var json=FuncScript.Helpers.FormatToJson(res);
        Assert.That(logger.GetLogContent().Trim(),Is.EqualTo("a"));
        Assert.That(FuncScript.Helpers.FormatToJson(FuncScript.Helpers.NormalizeDataType(new {r="5y",m="5z"})),Is.EqualTo(json));

        logger.Clear();
        nodes[0].Expression = "2 log 'b'";
        res = await session.EvaluateNodeAsync("dl");
        json=FuncScript.Helpers.FormatToJson(res);
        Assert.That(logger.GetLogContent().Trim(),Is.EqualTo("b"));
        
        Assert.That(FuncScript.Helpers.FormatToJson(FuncScript.Helpers.NormalizeDataType(new {r="2y",m="2z"})),Is.EqualTo(json));

    }
    [Test]
    public async Task EvaluateFileFunctionWithEmptyPath_ShouldReturnErrorInPProperty()
    {
        // The expression creates x = file(''),
        // then attempts to format x via p = f'{x}',
        // finally returns an object { p }.
        var expression = "{ x: file(''); p: f'{x}'; } { p }";

        // Prepare our single ExecutionNode.
        var node = new ExecutionNode
        {
            Name = "testNode",
            Expression = expression,
            ExpressionType = ExpressionType.FuncScript,
            Children = new ExecutionNode[0]
        };

        // Build the session.
        var session = new ExecutionSession(new[] { node },  null);

        // Evaluate the node by name.
        var result = await session.EvaluateNodeAsync("testNode");

        // We expect an object with one key: "p".
        Assert.That(result, Is.InstanceOf<KeyValueCollection>(), 
            "Result should be an FsObject (key-value structure in FuncScript).");

        var fsObj = (KeyValueCollection)result;
        Assert.That(fsObj.IsDefined("p"), 
            "Returned object should contain a key 'p'.");

        var pValue = fsObj.Get("p");
        Assert.That(pValue, Is.InstanceOf<FsError>(), 
            "The 'p' value should be an FsError due to file('') being invalid.");
    }
}
