using funcscript.block;
using funcscript.model;
using funcscript.model;
namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        static int GetUnit(KeyValueCollection provider, string exp, int index, out ExpressionBlock expBlock,
            out ParseNode parseNode, List<SyntaxErrorData> syntaxErrors)
        {
            ParseNode nodeUnit;
            parseNode = null;
            expBlock = null;
            int i;

            //get string
            i = GetStringTemplate(provider, exp, index, out var template, out nodeUnit, syntaxErrors);
            if (i > index)
            {
                parseNode = nodeUnit;
                expBlock = template;
                expBlock.SetContext(provider);
                expBlock.CodePos = index;
                expBlock.CodeLength = i - index;
                return i;
            }

            //get string 
            i = GetSimpleString(provider, exp, index, out var str, out nodeUnit, syntaxErrors);
            if (i > index)
            {
                parseNode = nodeUnit;
                expBlock = new LiteralBlock(str);
                expBlock.SetContext(provider);
                expBlock.CodePos = index;
                expBlock.CodeLength = i - index;
                return i;
            }

            //get number
            i = GetNumber(provider, exp, index, out var numberVal, out nodeUnit, syntaxErrors);
            if (i > index)
            {
                parseNode = nodeUnit;
                expBlock = new LiteralBlock(numberVal);
                expBlock.SetContext(provider);
                expBlock.CodePos = index;
                expBlock.CodeLength = i - index;
                return i;
            }

            //list expression
            i = GetListExpression(provider, exp, index, out var lst, out nodeUnit, syntaxErrors);
            if (i > index)
            {
                parseNode = nodeUnit;
                expBlock = lst;
                expBlock.SetContext(provider);
                expBlock.CodePos = index;
                expBlock.CodeLength = i - index;
                return i;
            }

            //kvc expression
            i = GetKvcExpression(provider, false, exp, index, out var json, out nodeUnit, syntaxErrors);
            if (i > index)
            {
                parseNode = nodeUnit;
                expBlock = json;
                expBlock.SetContext(provider);
                expBlock.CodePos = index;
                expBlock.CodeLength = i - index;
                return i;
            }

            i = GetCaseExpression(provider, exp, i, out var caseExp, out var caseNode, syntaxErrors);
            if (i > index)
            {
                parseNode = caseNode;
                expBlock = caseExp;
                expBlock.SetContext(provider);
                expBlock.CodePos = index;
                expBlock.CodeLength = i - index;
                return i;
            }

            i = GetSwitchExpression(provider, exp, i, out var switchExp, out var switchNode, syntaxErrors);
            if (i > index)
            {
                parseNode = switchNode;
                expBlock = switchExp;
                expBlock.SetContext(provider);
                expBlock.CodePos = index;
                expBlock.CodeLength = i - index;
                return i;
            }

            //expression function
            i = GetLambdaExpression(provider, exp, index, out var ef, out nodeUnit, syntaxErrors);
            if (i > index)
            {
                parseNode = nodeUnit;
                expBlock = new LiteralBlock(ef);
                expBlock.SetContext(provider);
                expBlock.CodePos = index;
                expBlock.CodeLength = i - index;
                return i;
            }

            //null, true, false
            i = GetKeyWordLiteral(provider, exp, index, out var kw, out nodeUnit);
            if (i > index)
            {
                parseNode = nodeUnit;
                expBlock = new LiteralBlock(kw);
                expBlock.SetContext(provider);
                expBlock.CodePos = index;
                expBlock.CodeLength = i - index;
                return i;
            }

            //get identifier
            i = GetIdentifier(provider, exp, index,true, out var ident, out var identLower,out var parentRef, out nodeUnit);
            if (i > index)
            {
                parseNode = nodeUnit;
                expBlock = new ReferenceBlock(ident,identLower,parentRef);
                expBlock.SetContext(provider);
                expBlock.CodePos = index;
                expBlock.CodeLength = i - index;
                return i;
            }

            i = GetExpInParenthesis(provider, exp, index, out expBlock, out nodeUnit, syntaxErrors);
            if (i > index)
            {
                parseNode = nodeUnit;
                expBlock.SetContext(provider);
                expBlock.CodePos = index;
                expBlock.CodeLength = i - index;
                return i;
            }

            //get prefix operator
            i = GetPrefixOperator(provider, exp, index, out var prefixOp, out var prefixOpNode, syntaxErrors);
            if (i > index)
            {
                expBlock = prefixOp;
                parseNode = prefixOpNode;
                expBlock.CodePos = index;
                expBlock.CodeLength = i - index;
                return i;
            }

            return index;
        }
    }
}