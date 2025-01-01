using funcscript.block;
using funcscript.model;
namespace funcscript.core
{
    public partial class FuncScriptParser
    {

        static ExpressionBlockResult GetInfixExpressionSingleOp(ParseContext context, int level, string[] candidates, int index)
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
                        var result = GetCallAndMemberAccess(context, i);
                        prog = result.Block;
                        parseNode = result.ParseNode;
                        i2 = result.NextIndex;
                    }
                    else
                    {
                        var result = GetInfixExpressionSingleOp(context, level - 1, s_operatorSymbols[level - 1], i);
                        prog = result.Block;
                        parseNode = result.ParseNode;
                        i2 = result.NextIndex;
                    }

                    if (i2 == i)
                        return new ExpressionBlockResult(null, null, i);

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

                var operands = new List<ExpressionBlock>();
                var operandNodes = new List<ParseNode>();
                operands.Add(prog);
                operandNodes.Add(parseNode);
                while (true)
                {
                    ExpressionBlock nextOperand;
                    ParseNode nextOperandNode;
                    if (level == 0)
                    {
                        var nextOperandResult = GetCallAndMemberAccess(context, i);
                        nextOperand = nextOperandResult.Block;
                        nextOperandNode = nextOperandResult.ParseNode;
                        i2 = nextOperandResult.NextIndex;
                    }
                    else
                    {
                        var nextOperandResult = GetInfixExpressionSingleOp(context, level - 1, s_operatorSymbols[level - 1], i);
                        nextOperand = nextOperandResult.Block;
                        nextOperandNode = nextOperandResult.ParseNode;
                        i2 = nextOperandResult.NextIndex;
                    }
                    
                    if (i2 == i)
                        return new ExpressionBlockResult(null, null, indexBeforeOperator);
                    
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
                    var func = context.ReferenceProvider.Get(symbol);
                    if (symbol == "|")
                    {
                        if (operands.Count > 2)
                        {
                            context.SyntaxErrors.Add(new SyntaxErrorData(i, 0, "Only two parameters expected for | "));
                            return new ExpressionBlockResult(null, null, i);
                        }

                        prog = new ListExpression
                        {
                            ValueExpressions = operands.ToArray(),
                            CodePos = prog.CodePos,
                            CodeLength = operands[^1].CodePos + operands[^1].CodeLength - prog.CodeLength
                        };
                        prog.SetContext(context.ReferenceProvider);

                        parseNode = new ParseNode(ParseNodeType.InfixExpression, parseNode!.Pos,
                            operandNodes[^1].Pos + operandNodes[^1].Length - parseNode.Length);
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
                        prog.SetContext(context.ReferenceProvider);
                        parseNode = new ParseNode(ParseNodeType.InfixExpression, parseNode!.Pos,
                            operandNodes[^1].Pos + operandNodes[^1].Length - parseNode.Length);
                    }
                }
            }
            return new ExpressionBlockResult(prog, parseNode, i);
        }
    }
}
