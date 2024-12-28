using funcscript.block;
using funcscript.model;
namespace funcscript.core
{
    public partial class FuncScriptParser
    {

        static ExpressionBlockResult GetInfixExpressionSingleLevel(ParseContext context, int level, string[] candidates, int index)
        {
            ExpressionBlock prog = null;
            ParseNode parseNode = null;
            var i = index;

            while (true)
            {
                int i2;
                IFsFunction oper = null;
                ParseNode operatorNode = null;
                string symbol = null;

                if (prog == null) //if we parsing the first operand
                {
                    //get an infix with one level higher or call expression when we are parsing for highest precedence operators
                    if (level == 0)
                    {
                        var result = GetInfixFunctionCall(context, i);
                        prog = result.Program;
                        parseNode = result.ParseNode;
                        i2 = result.NextIndex;
                    }
                    else
                    {
                        var result = GetInfixExpressionSingleLevel(context, level - 1, s_operatorSymols[level - 1], i);
                        prog = result.Expression;
                        parseNode = result.Node;
                        i2 = result.NextIndex;
                    }

                    if (i2 == i)
                        return new ExpressionBlockResult(prog, parseNode, i);

                    i = SkipSpace(context, i2).NextIndex;
                    continue;
                }

                var indexBeforeOperator = i;
                var operatorResult = GetOperator(context, candidates, i);
                symbol = operatorResult.MatchedOp;
                oper = operatorResult.Oper;
                operatorNode = operatorResult.ParseNode;
                i2 = operatorResult.NextIndex;

                if (i2 == i)
                    break;

                i = SkipSpace(context, i2).NextIndex;

                var operands = new List<ExpressionBlock> { prog };
                var operandNodes = new List<ParseNode> { parseNode };

                while (true)
                {
                    ExpressionBlock nextOperand;
                    ParseNode nextOperandNode;
                    if (level == 0)
                    {
                        var result = GetInfixFunctionCall(context, i);
                        nextOperand = result.Program;
                        nextOperandNode = result.ParseNode;
                        i2 = result.NextIndex;
                    }
                    else
                    {
                        var result = GetInfixExpressionSingleLevel(context, level - 1, s_operatorSymols[level - 1], i);
                        nextOperand = result.Expression;
                        nextOperandNode = result.Node;
                        i2 = result.NextIndex;
                    }

                    if (i2 == i)
                        return new ExpressionBlockResult(prog, parseNode, indexBeforeOperator);

                    operands.Add(nextOperand);
                    operandNodes.Add(nextOperandNode);
                    i = SkipSpace(context, i2).NextIndex;

                    i2 = GetLiteralMatch(context, i, symbol).NextIndex;
                    if (i2 == i)
                        break;
                    i = SkipSpace(context, i2).NextIndex;
                }

                if (operands.Count > 1)
                {
                    var func = context.Provider.Get(symbol);
                    if (symbol == "|")
                    {
                        if (operands.Count > 2)
                        {
                            context.SyntaxErrors.Add(new SyntaxErrorData(i, 0, "Only two parameters expected for | "));
                            return new ExpressionBlockResult(prog, parseNode, i);
                        }

                        prog = new ListExpression
                        {
                            ValueExpressions = operands.ToArray(),
                            CodePos = prog.CodePos,
                            CodeLength = operands[^1].CodePos + operands[^1].CodeLength - prog.CodeLength
                        };
                        prog.SetContext(context.Provider);
                        parseNode = new ParseNode(ParseNodeType.InfixExpression, parseNode.Pos, operandNodes[^1].Pos + operandNodes[^1].Length - parseNode.Length);
                    }
                    else
                    {
                        prog = new FunctionCallExpression
                        {
                            Function = new LiteralBlock(func),
                            Parameters = operands.ToArray(),
                            CodePos = prog.CodePos,
                            CodeLength = operands[^1].CodePos + operands[^1].CodeLength - prog.CodeLength
                        };
                        prog.SetContext(context.Provider);
                        parseNode = new ParseNode(ParseNodeType.InfixExpression, parseNode.Pos, operandNodes[^1].Pos + operandNodes[^1].Length - parseNode.Length);
                    }
                }
            }

            return new ExpressionBlockResult(prog, parseNode, i);
        }
    }
}