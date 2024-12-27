using funcscript.block;
using funcscript.model;
namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        static int GetFunctionCallParametersList(KeyValueCollection provider, ExpressionBlock func, String exp, int index,
            out ExpressionBlock prog, out ParseNode parseNode, List<SyntaxErrorData> syntaxErrors)
        {
            var i = GetFunctionCallParametersList(provider, "(", ")", func, exp, index, out prog, out parseNode,
                syntaxErrors);
            if (i == index)
                return GetFunctionCallParametersList(provider, "[", "]", func, exp, index, out prog, out parseNode,
                    syntaxErrors);
            return i;
        }

        static int GetFunctionCallParametersList(KeyValueCollection provider, String openBrance, String closeBrance,
            ExpressionBlock func, String exp, int index, out ExpressionBlock prog, out ParseNode parseNode,
            List<SyntaxErrorData> syntaxErrors)
        {
            parseNode = null;
            prog = null;

            //make sure we have open brace
            var i = SkipSpace(exp, index);
            var i2 = GetLiteralMatch(exp, i, openBrance);
            if (i == i2)
                return index; //we didn't find '('
            i = i2;
            var pars = new List<ExpressionBlock>();
            var parseNodes = new List<ParseNode>();
            //lets get first parameter
            i = SkipSpace(exp, i);
            i2 = GetExpression(provider, exp, i, out var par1, out var parseNode1, syntaxErrors);
            if (i2 > i)
            {
                i = i2;
                pars.Add(par1);
                parseNodes.Add(parseNode1);
                do
                {
                    i2 = SkipSpace(exp, i);
                    if (i2 >= exp.Length || exp[i2++] != ',') //stop collection of parameters if there is no ','
                        break;
                    i = i2;
                    i = SkipSpace(exp, i);
                    i2 = GetExpression(provider, exp, i, out var par2, out var parseNode2, syntaxErrors);
                    if (i2 == i)
                    {
                        syntaxErrors.Add(new SyntaxErrorData(i, 0, "Parameter for call expected"));
                        return index;
                    }

                    i = i2;
                    pars.Add(par2);
                    parseNodes.Add(parseNode2);
                } while (true);
            }

            i = SkipSpace(exp, i);
            i2 = GetLiteralMatch(exp, i, closeBrance);
            if (i2 == i)
            {
                syntaxErrors.Add(new SyntaxErrorData(i, 0, $"'{closeBrance}' expected"));
                return index;
            }

            i = i2;

            prog = new FunctionCallExpression
            {
                Function = func,
                Parameters = pars.ToArray(),
                CodePos = func.CodePos,
                CodeLength = i - func.CodePos,
            };
            prog.SetContext(provider);
            parseNode = new ParseNode(ParseNodeType.FunctionParameterList, index, i - index, parseNodes);
            return i;
        }
    }
}