using funcscript.block;
using funcscript.model;
using funcscript.model;
namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        static ExpressionBlockResult GetUnit(ParseContext context, int index)
        {
            ParseNode parseNode = null;
            ExpressionBlock expBlock = null;
            int i;

            //get string
            var templateResult = GetStringTemplate(context, index);
            i = templateResult.NextIndex;
            if (i > index)
            {
                parseNode = templateResult.ParseNode;
                expBlock = templateResult.Block;
                expBlock.CodePos = index;
                expBlock.CodeLength = i - index;
                return new ExpressionBlockResult(expBlock, parseNode, i);
            }

            //get string 
            var simpleStrResult = GetSimpleString(context, index);
            i = simpleStrResult.NextIndex;
            if (i > index)
            {
                parseNode = simpleStrResult.ParseNode;
                expBlock = new LiteralBlock(simpleStrResult.Str);
                expBlock.CodePos = index;
                expBlock.CodeLength = i - index;
                return new ExpressionBlockResult(expBlock, parseNode, i);
            }

            //get number
            var numberResult = GetNumber(context, index);
            i = numberResult.NextIndex;
            if (i > index)
            {
                parseNode = numberResult.ParseNode;
                expBlock = new LiteralBlock(numberResult.Number);
                expBlock.CodePos = index;
                expBlock.CodeLength = i - index;
                return new ExpressionBlockResult(expBlock, parseNode, i);
            }

            //list expression
            var listExprResult = GetListExpression(context, index);
            i = listExprResult.NextIndex;
            if (i > index)
            {
                parseNode = listExprResult.ParseNode;
                expBlock = listExprResult.Block;
                expBlock.CodePos = index;
                expBlock.CodeLength = i - index;
                return new ExpressionBlockResult(expBlock, parseNode, i);
            }

            //kvc expression
            var kvcExprResult = GetKvcExpression(context, index);
            i = kvcExprResult.NextIndex;
            if (i > index)
            {
                parseNode = kvcExprResult.ParseNode;
                expBlock = kvcExprResult.Block;
                expBlock.CodePos = index;
                expBlock.CodeLength = i - index;
                return new ExpressionBlockResult(expBlock, parseNode, i);
            }

            var caseExprResult = GetCaseExpression(context, i);
            i = caseExprResult.NextIndex;
            if (i > index)
            {
                parseNode = caseExprResult.ParseNode;
                expBlock = caseExprResult.Block;
                expBlock.CodePos = index;
                expBlock.CodeLength = i - index;
                return new ExpressionBlockResult(expBlock, parseNode, i);
            }

            var switchExprResult = GetSwitchExpression(context, i);
            i = switchExprResult.NextIndex;
            if (i > index)
            {
                parseNode = switchExprResult.ParseNode;
                expBlock = switchExprResult.Block;
                expBlock.CodePos = index;
                expBlock.CodeLength = i - index;
                return new ExpressionBlockResult(expBlock, parseNode, i);
            }

            //expression function
            var lambdaExprResult = GetLambdaExpression(context, index);
            i = lambdaExprResult.NextIndex;
            if (i > index)
            {
                parseNode = lambdaExprResult.ParseNode;
                expBlock = new LiteralBlock(lambdaExprResult.Block);
                expBlock.CodePos = index;
                expBlock.CodeLength = i - index;
                return new ExpressionBlockResult(expBlock, parseNode, i);
            }

            //null, true, false
            var keywordLiteralResult = GetKeyWordLiteral(context, index);
            i = keywordLiteralResult.NextIndex;
            if (i > index)
            {
                parseNode = keywordLiteralResult.ParseNode;
                expBlock = new LiteralBlock(keywordLiteralResult.Literal);
                expBlock.CodePos = index;
                expBlock.CodeLength = i - index;
                return new ExpressionBlockResult(expBlock, parseNode, i);
            }

            //get identifier
            var identResult = GetIdentifier(context, index, true);
            i = identResult.NextIndex;
            if (i > index)
            {
                parseNode = identResult.ParseNode;
                expBlock = new ReferenceBlock(identResult.Iden, identResult.IdenLower, identResult.ParentRef);
                expBlock.CodePos = index;
                expBlock.CodeLength = i - index;
                return new ExpressionBlockResult(expBlock, parseNode, i);
            }

            var expInParenResult = GetExpInParenthesis(context, index);
            i = expInParenResult.NextIndex;
            if (i > index)
            {
                parseNode = expInParenResult.ParseNode;
                expBlock = expInParenResult.Block;
                expBlock.CodePos = index;
                expBlock.CodeLength = i - index;
                return new ExpressionBlockResult(expBlock, parseNode, i);
            }

            //get prefix operator
            var prefixOpResult = GetPrefixOperator(context, index);
            i = prefixOpResult.NextIndex;
            if (i > index)
            {
                parseNode = prefixOpResult.ParseNode;
                expBlock = prefixOpResult.Block;
                expBlock.CodePos = index;
                expBlock.CodeLength = i - index;
                return new ExpressionBlockResult(expBlock, parseNode, i);
            }

            return new ExpressionBlockResult(expBlock, parseNode, index);
        }
    }
}