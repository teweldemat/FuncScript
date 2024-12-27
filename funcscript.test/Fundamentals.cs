using System;
using System.Collections.Generic;
using System.Linq;
using funcscript.block;
using funcscript.core;
using funcscript.funcs.math;
using funcscript.model;
using NUnit.Framework;

namespace funcscript.test;

public class Fundamentals
{
    [Test]
    public void TestIntLiteral()
    {
        const string expStr = "5";
        var p = new DefaultFsDataProvider();
        var errors = new List<FuncScriptParser.SyntaxErrorData>();
        var exp = FuncScriptParser.Parse(p, expStr, out var node, errors);
        Assert.IsNotNull(exp);
        Assert.That(errors, Is.Empty);
        Assert.That(node.NodeType, Is.EqualTo(FuncScriptParser.ParseNodeType.LiteralInteger));
        AssertLiteralBlock(exp, 5);

        var res = exp.Evaluate();
        Assert.That(res, Is.EqualTo(5));
    }

    [Test]
    public void TestIntOp()
    {
        const string expStr = "5+3";
        var p = new DefaultFsDataProvider();
        var errors = new List<FuncScriptParser.SyntaxErrorData>();
        var exp = FuncScriptParser.Parse(p, expStr, out var node, errors);
        Assert.IsNotNull(exp);
        Assert.That(errors, Is.Empty);
        
        Assert.That(node.NodeType, Is.EqualTo(FuncScriptParser.ParseNodeType.InfixExpression));
        AssertLiteralFunctionCallBlock<AddFunction>(
            exp,
            b => AssertLiteralBlock(b, 5),
            b => AssertLiteralBlock(b, 3)
        );
        
        var res = exp.Evaluate();
        Assert.That(res, Is.EqualTo(8));
    }

    [Test]
    public void TestKv()
    {
        const string expStr = "{x:5}";
        var p = new DefaultFsDataProvider();
        var errors = new List<FuncScriptParser.SyntaxErrorData>();
        var exp = FuncScriptParser.Parse(p, expStr, out var node, errors);
        
        Assert.IsNotNull(exp);
        Assert.That(errors, Is.Empty);
        
        Assert.That(node.NodeType, Is.EqualTo(FuncScriptParser.ParseNodeType.KeyValueCollection));
        Assert.That(exp is KvcExpression);
        var block = (KvcExpression)exp;
        Assert.That(block.KeyValues.Count, Is.EqualTo(1));
        var kv = block.KeyValues.First();
        Assert.That(kv.Key, Is.EqualTo("x"));
        AssertLiteralBlock(kv.ValueExpression, 5);

        var res = exp.Evaluate();
        Assert.That(res is KeyValueCollection);
        var resKv = (KeyValueCollection)res;
        var all = resKv.GetAll();
        Assert.That(all.Count(), Is.EqualTo(1));
        var pair = all.First();
        Assert.That(pair.Key, Is.EqualTo("x"));
        Assert.That(pair.Value, Is.EqualTo(5));
    }

    [Test]
    public void TestList()
    {
        const string expStr = "[5]";
        var p = new DefaultFsDataProvider();
        var errors = new List<FuncScriptParser.SyntaxErrorData>();
        var exp = FuncScriptParser.Parse(p, expStr, out var node, errors);
        
        Assert.IsNotNull(exp);
        Assert.That(errors, Is.Empty);
        
        Assert.That(node.NodeType, Is.EqualTo(FuncScriptParser.ParseNodeType.List));
        Assert.That(exp is ListExpression);
        var listBlock = (ListExpression)exp;
        Assert.That(listBlock.Length, Is.EqualTo(1));
        AssertLiteralBlock(listBlock.ValueExpressions.First(), 5);

        var res = exp.Evaluate();
        Assert.That(res is FsList);
        var resList = (FsList)res;
        Assert.That(resList.Length, Is.EqualTo(1));
        var item = resList.First();
        Assert.That(item, Is.EqualTo(5));
    }
    
    [Test]
    public void TestKvRef()
    {
        const string expStr = "{x:5,y:x+2}";
        var p = new DefaultFsDataProvider();
        var errors = new List<FuncScriptParser.SyntaxErrorData>();
        var exp = FuncScriptParser.Parse(p, expStr, out var node, errors);

        Assert.IsNotNull(exp);
        Assert.That(errors, Is.Empty);

        Assert.That(node.NodeType, Is.EqualTo(FuncScriptParser.ParseNodeType.KeyValueCollection));
        Assert.That(exp is KvcExpression);
        var block = (KvcExpression)exp;
        Assert.That(block.KeyValues.Count, Is.EqualTo(2));
        Assert.IsNull(block.singleReturn);

        var kv0 = block.KeyValues[0];
        Assert.That(kv0.Key, Is.EqualTo("x"));
        AssertLiteralBlock(kv0.ValueExpression, 5);

        var kv1 = block.KeyValues[1];
        Assert.That(kv1.Key, Is.EqualTo("y"));
        AssertLiteralFunctionCallBlock<AddFunction>(
            kv1.ValueExpression,
            b => AssertReferenceBlock(b, "x"),
            b => AssertLiteralBlock(b, 2)
        );

        var result = exp.Evaluate();
        Assert.That(result is KeyValueCollection);
        var kvResult = (KeyValueCollection)result;
        Assert.That(kvResult.Get("x"), Is.EqualTo(5));
        Assert.That(kvResult.Get("y"), Is.EqualTo(7));
    }

    void AssertLiteralBlock(ExpressionBlock block, object expectedValueOrType)
    {
        Assert.That(block is LiteralBlock);
        var literal = (LiteralBlock)block;
        if (expectedValueOrType is Type t)
        {
            Assert.That(literal.Value, Is.InstanceOf(t));
        }
        else
        {
            Assert.That(literal.Value, Is.EqualTo(expectedValueOrType));
        }
    }

    void AssertReferenceBlock(ExpressionBlock block, string expectedRefName)
    {
        Assert.That(block is ReferenceBlock);
        var rb = (ReferenceBlock)block;
        Assert.That(rb.Name, Is.EqualTo(expectedRefName));
    }
    void AssertLiteralFunctionCallBlock<FType>(ExpressionBlock block, params Action<ExpressionBlock>[] assertParams)
        where FType : IFsFunction
    {
        Assert.That(block is FunctionCallExpression);
        var f = (FunctionCallExpression)block;
        AssertLiteralBlock(f.Function, typeof(FType));
        var fval = (FType)((LiteralBlock)f.Function).Value;
        for (int i = 0; i < assertParams.Length; i++)
        {
            var a = assertParams[i];
            Assert.That(i < f.Parameters.Length, $"Parameter {i} not passed to {fval.Symbol}");
            a(f.Parameters[i]);
        }
    }

}