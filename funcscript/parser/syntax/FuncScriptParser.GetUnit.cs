using funcscript.block;
using funcscript.model;
using funcscript.model;
namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        static ParseResult GetUnit(ParseContext context, int index)
        {
            ParseNode parseNode = null;
            ExpressionBlock expBlock = null;
            int i;

            //get string
            var templateResult = GetStringTemplate(context, index);
            i = templateResult.NextIndex;
            if (i > index)
            {
                parseNode = templateResult.Node;
                expBlock = templateResult.Expression;
                expBlock.SetContext(context.Provider);
                expBlock.CodePos = index;
                expBlock.CodeLength = i - index;
                return new ParseResult(expBlock, parseNode, i);
            }

            //get string 
            var simpleStrResult = GetSimpleString(context, index);
            i = simpleStrResult.NextIndex;
            if (i > index)
            {
                parseNode = simpleStrResult.Node;
                expBlock = new LiteralBlock(simpleStrResult.Str);
                expBlock.SetContext(context.Provider);
                expBlock.CodePos = index;
                expBlock.CodeLength = i - index;
                return new ParseResult(expBlock, parseNode, i);
            }

            //get number
            var numberResult = GetNumber(context, index);
            i = numberResult.NextIndex;
            if (i > index)
            {
                parseNode = numberResult.ParseNode;
                expBlock = new LiteralBlock(numberResult.Number);
                expBlock.SetContext(context.Provider);
                expBlock.CodePos = index;
                expBlock.CodeLength = i - index;
                return new ParseResult(expBlock, parseNode, i);
            }

            //list expression
            var listExprResult = GetListExpression(context, index);
            i = listExprResult.NextIndex;
            if (i > index)
            {
                parseNode = listExprResult.ParseNode;
                expBlock = listExprResult.ListExpr;
                expBlock.SetContext(context.Provider);
                expBlock.CodePos = index;
                expBlock.CodeLength = i - index;
                return new ParseResult(expBlock, parseNode, i);
            }

            //kvc expression
            var kvcExprResult = GetKvcExpression(context, false, index);
            i = kvcExprResult.NextIndex;
            if (i > index)
            {
                parseNode = kvcExprResult.ParseNode;
                expBlock = kvcExprResult.KvcExpr;
                expBlock.SetContext(context.Provider);
                expBlock.CodePos = index;
                expBlock.CodeLength = i - index;
                return new ParseResult(expBlock, parseNode, i);
            }

            var caseExprResult = GetCaseExpression(context, i);
            i = caseExprResult.NextIndex;
            if (i > index)
            {
                parseNode = caseExprResult.Node;
                expBlock = caseExprResult.Expression;
                expBlock.SetContext(context.Provider);
                expBlock.CodePos = index;
                expBlock.CodeLength = i - index;
                return new ParseResult(expBlock, parseNode, i);
            }

            var switchExprResult = GetSwitchExpression(context, i);
            i = switchExprResult.NextIndex;
            if (i > index)
            {
                parseNode = switchExprResult.ParseNode;
                expBlock = switchExprResult.Prog;
                expBlock.SetContext(context.Provider);
                expBlock.CodePos = index;
                expBlock.CodeLength = i - index;
                return new ParseResult(expBlock, parseNode, i);
            }

            //expression function
            var lambdaExprResult = GetLambdaExpression(context, index);
            i = lambdaExprResult.NextIndex;
            if (i > index)
            {
                parseNode = lambdaExprResult.Node;
                expBlock = new LiteralBlock(lambdaExprResult.ExpressionFunction);
                expBlock.SetContext(context.Provider);
                expBlock.CodePos = index;
                expBlock.CodeLength = i - index;
                return new ParseResult(expBlock, parseNode, i);
            }

            //null, true, false
            var keywordLiteralResult = GetKeyWordLiteral(context, index);
            i = keywordLiteralResult.NextIndex;
            if (i > index)
            {
                parseNode = keywordLiteralResult.ParseNode;
                expBlock = new LiteralBlock(keywordLiteralResult.Literal);
                expBlock.SetContext(context.Provider);
                expBlock.CodePos = index;
                expBlock.CodeLength = i - index;
                return new ParseResult(expBlock, parseNode, i);
            }

            //get identifier
            var identResult = GetIdentifier(context, index, true);
            i = identResult.NextIndex;
            if (i > index)
            {
                parseNode = identResult.ParseNode;
                expBlock = new ReferenceBlock(identResult.Iden, identResult.IdenLower, identResult.ParentRef);
                expBlock.SetContext(context.Provider);
                expBlock.CodePos = index;
                expBlock.CodeLength = i - index;
                return new ParseResult(expBlock, parseNode, i);
            }

            var expInParenResult = GetExpInParenthesis(context, index);
            i = expInParenResult.NextIndex;
            if (i > index)
            {
                parseNode = expInParenResult.Node;
                expBlock = expInParenResult.Expression;
                expBlock.SetContext(context.Provider);
                expBlock.CodePos = index;
                expBlock.CodeLength = i - index;
                return new ParseResult(expBlock, parseNode, i);
            }

            //get prefix operator
            var prefixOpResult = GetPrefixOperator(context, index);
            i = prefixOpResult.NextIndex;
            if (i > index)
            {
                parseNode = prefixOpResult.Node;
                expBlock = prefixOpResult.Expression;
                expBlock.CodePos = index;
                expBlock.CodeLength = i - index;
                return new ParseResult(expBlock, parseNode, i);
            }

            return new ParseResult(expBlock, parseNode, index);
        }
    }
}