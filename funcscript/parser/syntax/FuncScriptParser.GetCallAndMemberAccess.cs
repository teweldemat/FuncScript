using funcscript.block;
namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        static ExpressionBlockResult GetCallAndMemberAccess(ParseContext context, int index)
        {
            ExpressionBlock prog = null;
            var i1 = SkipSpace(context, index).NextIndex;
            var (theUnit,parseNode,i) = GetUnit(context, i1);
            if (i == index)
                return new ExpressionBlockResult(prog, parseNode, index);

            do
            {
                var (funcCall, nodeParList,i2) = GetFunctionCallParametersList(context, theUnit, i);
                if (i2 > i)
                {
                    i = i2;
                    theUnit = funcCall;
                    parseNode = new ParseNode(ParseNodeType.FunctionCall, index, i - index, new[] { parseNode, nodeParList });
                    continue;
                }

                (var memberAccess, var nodeMemberAccess,i2) = GetMemberAccess(context, theUnit, i);
                if (i2 > i)
                {
                    i = i2;
                    theUnit = memberAccess;
                    parseNode = new ParseNode(ParseNodeType.MemberAccess, index, i - index, new[] { parseNode, nodeMemberAccess });
                    continue;
                }

                (var kvc,var nodeKvc,i2) = GetKvcExpression(context, false, i);
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
                    theUnit.SetContext(context.Provider);
                    parseNode = new ParseNode(ParseNodeType.Selection, index, i - index, new[] { parseNode, nodeKvc });
                    continue;
                }

                prog = theUnit;
                return new ExpressionBlockResult(prog, parseNode, i);
            } while (true);
        }
    }
}