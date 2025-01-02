using System.Collections.Generic;
using funcscript.core;
using NUnit.Framework;

namespace funcscript.test;

public class KeyValueWithReturnTest
{
    [Test]
    public void KeyValueWithReturnParseCorrectly()
    {
        const string kv = "x:5";
        const string ret = "return x";
        var expression = $"{{{kv};{ret};}}";
        var syntaxErrors = new List<FuncScriptParser.SyntaxErrorData>();
        var context = new FuncScriptParser.ParseContext(new DefaultFsDataProvider(), expression, syntaxErrors);

        var (exp, parseNode, _) = FuncScriptParser.Parse(context);

        Assert.IsNotNull(parseNode);
        Assert.AreEqual(FuncScriptParser.ParseNodeType.KeyValueCollection, parseNode.NodeType);

        Assert.AreEqual(2, parseNode.Children.Count); // Expecting two top-level children: kv and ret;

        // Validate the first child: kv
        var keyValuePairNode = parseNode.Children[0];
        Assert.AreEqual(FuncScriptParser.ParseNodeType.KeyValuePair, keyValuePairNode.NodeType);
        Assert.AreEqual(1, keyValuePairNode.Pos);    // 'kv' starts at index 1
        Assert.AreEqual(kv.Length, keyValuePairNode.Length); // 'kv' is kv.Length characters long

        Assert.That(keyValuePairNode.Children.Count, Is.EqualTo(2));
        
        var keyNode = keyValuePairNode.Children[0];
        Assert.AreEqual(FuncScriptParser.ParseNodeType.Key, keyNode.NodeType);
        Assert.AreEqual(1, keyNode.Pos);    // Key starts at index 1
        Assert.AreEqual(1, keyNode.Length); // Key is 1 character long

        var valueNode = keyValuePairNode.Children[1];
        Assert.AreEqual(FuncScriptParser.ParseNodeType.LiteralInteger, valueNode.NodeType);
        Assert.AreEqual(1 + kv.IndexOf(':') + 1, valueNode.Pos); // Value starts after ':' in kv
        Assert.AreEqual(1, valueNode.Length); // Value is 1 character long

        // Validate the second child: ret
        var returnNode = parseNode.Children[1];
        Assert.AreEqual(FuncScriptParser.ParseNodeType.ReturnExpression, returnNode.NodeType);
        Assert.AreEqual(1 + kv.Length + 1, returnNode.Pos); // 'ret' starts after 'kv;' in expression
        Assert.AreEqual(ret.Length, returnNode.Length); // 'ret' is ret.Length characters long

        Assert.That(returnNode.Children.Count, Is.EqualTo(2));
        
        var keyWordNode = returnNode.Children[0];
        Assert.AreEqual(FuncScriptParser.ParseNodeType.KeyWord, keyWordNode.NodeType);
        Assert.AreEqual(1 + kv.Length + 1, keyWordNode.Pos); // 'return' starts at the beginning of 'ret'
        Assert.AreEqual("return".Length, keyWordNode.Length); // 'return' is 6 characters long

        var identifierNode = returnNode.Children[1];
        Assert.AreEqual(FuncScriptParser.ParseNodeType.Identifier, identifierNode.NodeType);
        Assert.AreEqual(1 + kv.Length + 1 + "return".Length + 1, identifierNode.Pos); // 'x' starts after 'return ' in ret
        Assert.AreEqual(1, identifierNode.Length); // 'x' is 1 character long
    }
}