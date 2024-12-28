using funcscript.block;
using funcscript.model;
namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        static ParseResult GetFunctionCallParametersList(ParseContext context, ExpressionBlock func, int index)
        {
            var result = GetFunctionCallParametersList(context, "(", ")", func, index);
            if (result.NextIndex == index)
                return GetFunctionCallParametersList(context, "[", "]", func, index);
            return result;
        }

        public record FunctionCallParametersResult(ExpressionBlock Prog, ParseNode Node, int NextIndex) : ParseResult(Prog, Node, NextIndex);

        static FunctionCallParametersResult GetFunctionCallParametersList(ParseContext context, string openBrace, string closeBrace,
            ExpressionBlock func, int index)
        {
            ParseNode parseNode = null;
            ExpressionBlock prog = null;

            var i = SkipSpace(context, index).NextIndex;
            var i2 = GetLiteralMatch(context, i, openBrace).NextIndex;
            if (i == i2)
                return new FunctionCallParametersResult(null, null, index);

            i = i2;
            var pars = new List<ExpressionBlock>();
            var parseNodes = new List<ParseNode>();

            i = SkipSpace(context, i).NextIndex;
            var exprResult = GetExpression(context, i);

            if (exprResult.NextIndex > i)
            {
                i = exprResult.NextIndex;
                pars.Add(exprResult.Expression);
                parseNodes.Add(exprResult.Node);
                do
                {
                    i2 = SkipSpace(context, i).NextIndex;
                    if (i2 >= context.Expression.Length || context.Expression[i2++] != ',')
                        break;
                    i = i2;
                    i = SkipSpace(context, i).NextIndex;
                    exprResult = GetExpression(context, i);
                    if (exprResult.NextIndex == i)
                    {
                        context.Serrors.Add(new SyntaxErrorData(i, 0, "Parameter for call expected"));
                        return new FunctionCallParametersResult(null, null, index);
                    }

                    i = exprResult.NextIndex;
                    pars.Add(exprResult.Expression);
                    parseNodes.Add(exprResult.Node);
                } while (true);
            }

            i = SkipSpace(context, i).NextIndex;
            i2 = GetLiteralMatch(context, i, closeBrace).NextIndex;
            if (i2 == i)
            {
                context.Serrors.Add(new SyntaxErrorData(i, 0, $"'{closeBrace}' expected"));
                return new FunctionCallParametersResult(null, null, index);
            }

            i = i2;

            prog = new FunctionCallExpression
            {
                Function = func,
                Parameters = pars.ToArray(),
                CodePos = func.CodePos,
                CodeLength = i - func.CodePos,
            };
            prog.SetContext(context.Provider);
            parseNode = new ParseNode(ParseNodeType.FunctionParameterList, index, i - index, parseNodes);
            return new FunctionCallParametersResult(prog, parseNode, i);
        }
    }
}