using funcscript.block;
using funcscript.model;

namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        static int GetUnit(IFsDataProvider provider, string exp, int index, out ExpressionBlock prog,
            out ParseNode parseNode, List<SyntaxErrorData> serrors)
        {
            ParseNode nodeUnit;
            parseNode = null;
            prog = null;
            int i;

            //get string
            i = GetStringTemplate(provider, exp, index, out var template, out nodeUnit, serrors);
            if (i > index)
            {
                parseNode = nodeUnit;
                prog = template;
                prog.Provider = provider;
                prog.CodePos = index;
                prog.CodeLength = i - index;
                return i;
            }

            //get string 
            i = GetSimpleString(provider, exp, index, out var str, out nodeUnit, serrors);
            if (i > index)
            {
                parseNode = nodeUnit;
                prog = new LiteralBlock(str) { Provider = provider };
                prog.CodePos = index;
                prog.CodeLength = i - index;
                return i;
            }

            //get number
            i = GetNumber(provider, exp, index, out var numberVal, out nodeUnit, serrors);
            if (i > index)
            {
                parseNode = nodeUnit;
                prog = new LiteralBlock(numberVal) { Provider = provider };
                prog.CodePos = index;
                prog.CodeLength = i - index;
                return i;
            }

            //list expression
            i = GetListExpression(provider, exp, index, out var lst, out nodeUnit, serrors);
            if (i > index)
            {
                parseNode = nodeUnit;
                prog = lst;
                prog.Provider = provider;
                prog.CodePos = index;
                prog.CodeLength = i - index;
                return i;
            }

            //kvc expression
            i = GetKvcExpression(provider, false, exp, index, out var json, out nodeUnit, serrors);
            if (i > index)
            {
                parseNode = nodeUnit;
                prog = json;
                prog.Provider = provider;
                prog.CodePos = index;
                prog.CodeLength = i - index;
                return i;
            }

            i = GetCaseExpression(provider, exp, i, out var caseExp, out var caseNode, serrors);
            if (i > index)
            {
                parseNode = caseNode;
                prog = caseExp;
                prog.Provider = provider;
                prog.CodePos = index;
                prog.CodeLength = i - index;
                return i;
            }

            i = GetSwitchExpression(provider, exp, i, out var switchExp, out var switchNode, serrors);
            if (i > index)
            {
                parseNode = switchNode;
                prog = switchExp;
                prog.Provider = provider;
                prog.CodePos = index;
                prog.CodeLength = i - index;
                return i;
            }

            //expression function
            i = GetLambdaExpression(provider, exp, index, out var ef, out nodeUnit, serrors);
            if (i > index)
            {
                parseNode = nodeUnit;
                prog = new LiteralBlock(ef) { Provider = provider };
                prog.CodePos = index;
                prog.CodeLength = i - index;
                return i;
            }

            //null, true, false
            i = GetKeyWordLiteral(provider, exp, index, out var kw, out nodeUnit);
            if (i > index)
            {
                parseNode = nodeUnit;
                prog = new LiteralBlock(kw) { Provider = provider };
                prog.CodePos = index;
                prog.CodeLength = i - index;
                return i;
            }

            //get identifier
            i = GetIdentifier(provider, exp, index, out var ident, out var identLower, out nodeUnit);
            if (i > index)
            {
                parseNode = nodeUnit;
                prog = new ReferenceBlock(ident) { Provider = provider };
                prog.CodePos = index;
                prog.CodeLength = i - index;
                return i;
            }

            i = GetExpInParenthesis(provider, exp, index, out prog, out nodeUnit, serrors);
            if (i > index)
            {
                parseNode = nodeUnit;
                prog.Provider = provider;
                prog.CodePos = index;
                prog.CodeLength = i - index;
                return i;
            }

            //get prefix operator
            i = GetPrefixOperator(provider, exp, index, out var prefixOp, out var prefixOpNode, serrors);
            if (i > index)
            {
                prog = prefixOp;
                parseNode = prefixOpNode;
                prog.CodePos = index;
                prog.CodeLength = i - index;
                return i;
            }

            return index;
        }
    }
}