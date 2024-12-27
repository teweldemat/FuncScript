using funcscript.block;
using funcscript.model;

namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        static int GetCallAndMemberAccess(KeyValueCollection provider, string exp, int index, out ExpressionBlock prog,
            out ParseNode parseNode, List<SyntaxErrorData> syntaxErrors)
        {
            parseNode = null;
            prog = null;
            var i1 = SkipSpace(exp, index);
            var i = GetUnit(provider, exp, i1, out var theUnit, out parseNode, syntaxErrors);
            if (i == index)
                return index;

            do
            {
                //lets see if this is part of a function call
                var i2 = GetFunctionCallParametersList(provider, theUnit, exp, i, out var funcCall,
                    out var nodeParList, syntaxErrors);
                if (i2 > i)
                {
                    i = i2;
                    theUnit = funcCall;
                    parseNode = new ParseNode(ParseNodeType.FunctionCall, index, i - index,
                        new[] { parseNode, nodeParList });
                    continue;
                }

                i2 = GetMemberAccess(provider, theUnit, exp, i, out var memberAccess, out var nodeMemberAccess,
                    syntaxErrors);
                if (i2 > i)
                {
                    i = i2;
                    theUnit = memberAccess;
                    parseNode = new ParseNode(ParseNodeType.MemberAccess, index, i - index,
                        new[] { parseNode, nodeMemberAccess });
                    continue;
                }

                i2 = GetKvcExpression(provider, false, exp, i, out var kvc, out var nodeKvc, syntaxErrors);
                if (i2 > i)
                {
                    i = i2;
                    theUnit = new SelectorExpression
                    {
                        Source = theUnit,
                        Selector = kvc,
                        CodePos = i,
                        CodeLength = i2 - i
                    };
                    theUnit.SetContext(provider);
                    parseNode = new ParseNode(ParseNodeType.Selection, index, i - index, new[] { parseNode, nodeKvc });
                    continue;
                }

                prog = theUnit;
                return i;
            } while (true);
        }
    }
}