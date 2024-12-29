using System.Collections.Generic;
using funcscript.core;
using NUnit.Framework;

namespace funcscript.test;

public class IdentifierWithDigitParse
{
        [Test]
        public void IdentifierWithDigitParseTest()
        {
            var expression = "x2";
            var syntaxErrors = new List<FuncScriptParser.SyntaxErrorData>();
            var context = new FuncScriptParser.ParseContext(new DefaultFsDataProvider(), expression, syntaxErrors);

            var (exp, parseNode, _) = FuncScriptParser.Parse(context);

            Assert.IsNotNull(parseNode);
            Assert.AreEqual(FuncScriptParser.ParseNodeType.Identifier, parseNode.NodeType);

            Assert.AreEqual(0, parseNode.Pos);
            Assert.AreEqual(2, parseNode.Length);

            Assert.IsEmpty(parseNode.Children);
        }
    
}