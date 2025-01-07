using FuncScript.Block;
using FuncScript.Model;
namespace FuncScript.Core
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

                if (prog == null) //if we are parsing the first operand
                {
                    //get an infix with one level higher or call expression when we are parsing for highest precedence operators
                    if (level == 0)
                    {
                        var result = GetInfixFunctionCall(context, i);
                        prog = result.Block;
                        parseNode = result.ParseNode;
                        i2 = result.NextIndex;
                    }
                    else
                    {
                        var result = GetInfixExpressionSingleLevel(context, level - 1, s_operatorSymbols[level - 1], i);
                        prog = result.Block;
                        parseNode = result.ParseNode;
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
                operatorNode = operatorResult.ParseNode;
                i2 = operatorResult.NextIndex;

                if (i2 == i)
                    break;

                i = SkipSpace(context, i2).NextIndex;

                var operands = new List<ExpressionBlock> { prog };
                var infixComponentNodes = new List<ParseNode> { parseNode };

                while (true)
                {
                    ExpressionBlock nextOperand;
                    ParseNode nextOperandNode;
                    if (level == 0)
                    {
                        var result = GetInfixFunctionCall(context, i);
                        nextOperand = result.Block;
                        nextOperandNode = result.ParseNode;
                        i2 = result.NextIndex;
                    }
                    else
                    {
                        var result = GetInfixExpressionSingleLevel(context, level - 1, s_operatorSymbols[level - 1], i);
                        nextOperand = result.Block;
                        nextOperandNode = result.ParseNode;
                        i2 = result.NextIndex;
                    }

                    if (i2 == i)
                        return new ExpressionBlockResult(prog, parseNode, indexBeforeOperator);

                    operands.Add(nextOperand);
                    infixComponentNodes.Add(operatorNode);
                    infixComponentNodes.Add(nextOperandNode);
                    i = SkipSpace(context, i2).NextIndex;
                    operatorResult = GetOperator(context, new string[] { symbol }, i);
                    i2 = operatorResult.NextIndex;
                    if (i2 == i)
                        break;
                    operatorNode = operatorResult.ParseNode;
                    i = SkipSpace(context, i2).NextIndex;
                }

                if (operands.Count > 1)
                {
                    var func = context.ReferenceProvider.Get(symbol);
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
                            CodeLength = operands[^1].CodePos + operands[^1].CodeLength - prog.CodePos
                        };
                        parseNode = new ParseNode(ParseNodeType.InfixExpression, parseNode.Pos, infixComponentNodes[^1].Pos + infixComponentNodes[^1].Length - parseNode.Pos);
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
                        parseNode = new ParseNode(ParseNodeType.InfixExpression,
                            parseNode.Pos, infixComponentNodes[^1].Pos + infixComponentNodes[^1].Length - parseNode.Pos,
                            infixComponentNodes
                            );
                    }
                }
            }

            return new ExpressionBlockResult(prog, parseNode, i);
        }
    }
}
