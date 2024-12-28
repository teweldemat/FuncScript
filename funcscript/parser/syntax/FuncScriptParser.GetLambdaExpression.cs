using funcscript.block;
using funcscript.funcs.math;
using funcscript.model;
namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        record ParseLambdaExpressionResult(ExpressionFunction Block, ParseNode ParseNode, int NextIndex)
            :ParseResult(ParseNode,NextIndex);
        static ParseLambdaExpressionResult GetLambdaExpression(ParseContext context, int index)
        {
            ParseNode parseNode = null;
            ExpressionFunction func = null;
            
            var (parms,nodesParams,  i) = GetIdentifierList(context, index);
            if (i == index)
                return new ParseLambdaExpressionResult(func, parseNode, index);

            i = SkipSpace(context, i).NextIndex;
            if (i >= context.Expression.Length - 1) // we need two characters
                return new ParseLambdaExpressionResult(func, parseNode, index);

            var i2 = GetLiteralMatch(context, i, "=>").NextIndex;
            if (i2 == i)
            {
                context.SyntaxErrors.Add(new SyntaxErrorData(i, 0, "'=>' expected"));
                return new ParseLambdaExpressionResult(func, parseNode, index);
            }

            i += 2;
            i = SkipSpace(context, i).NextIndex;
            var parmsSet = new HashSet<string>(parms);


           (var defination,var nodeDefination, i2) = GetExpression(context, i);
            if (i2 == i)
            {
                context.SyntaxErrors.Add(new SyntaxErrorData(i, 0, "definition of lambda expression expected"));
                return new ParseLambdaExpressionResult(func, parseNode, index);
            }

            func = new ExpressionFunction(parms.ToArray(), defination);
            func.SetContext(context.ReferenceProvider);
            i = i2;
            parseNode = new ParseNode(ParseNodeType.LambdaExpression, index, i - index,
                new[] { nodesParams, nodeDefination });
            return new ParseLambdaExpressionResult(func, parseNode, i);
        }
    }

    public record ParseLambdaExpressionResult(ExpressionFunction ExpressionFunction, FuncScriptParser.ParseNode Node, int NextIndex);
}