using FuncScript.Block;
namespace FuncScript.Core
{
    public partial class FuncScriptParser
    {
        static ExpressionBlockResult GetCallAndMemberAccess(ParseContext context, int index)
        {
            ExpressionBlock prog = null;
            var i1 = SkipSpace(context, index).NextIndex;
            var (theUnit, parseNode, i) = GetUnit(context, i1);
            if (i == index)
                return new ExpressionBlockResult(prog, parseNode, index);

            do
            {
                var (funcCall, nodeParList, i2) = GetFunctionCallParametersList(context, theUnit, i);
                if (i2 > i)
                {
                    i = i2;
                    theUnit = funcCall;
                    theUnit.CodePos = parseNode.Pos;
                    theUnit.CodeLength=nodeParList.Pos-parseNode.Pos+nodeParList.Length;
                    parseNode = new ParseNode(ParseNodeType.FunctionCall, index, i - index, new[] { parseNode, nodeParList });
                    continue;
                }

                (var memberAccess, var nodeMemberAccess, i2) = GetMemberAccess(context, theUnit, i);
                if (i2 > i)
                {
                    i = i2;
                    theUnit = memberAccess;
                    parseNode = new ParseNode(ParseNodeType.MemberAccess, index, i - index, new[] { parseNode, nodeMemberAccess });
                    continue;
                }

                (var kvc, var nodeKvc, i2) = GetSelectKvcExpression(context, i);
                if (i2 > i)
                {
                    i = i2;
                    theUnit = new SelectorExpression
                    {
                        Source = theUnit,
                        Selector = kvc as KvcExpression,
                        CodePos = i,
                        CodeLength = i2 - i
                    };
                    parseNode = new ParseNode(ParseNodeType.Selection, index, i - index, new[] { parseNode, nodeKvc });
                    continue;
                }

                prog = theUnit;
                return new ExpressionBlockResult(prog, parseNode, i);
            } while (true);
        }
    }
}
