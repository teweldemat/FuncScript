using FuncScript.Block;
using FuncScript.Model;
using System.Collections.Generic;

namespace FuncScript.Core
{
    public partial class FuncScriptParser
    {
        static ExpressionBlockResult GetInfixFunctionCall(ParseContext context, int index)
        {
            int originalIndex = index;

            var leftResult = GetCallAndMemberAccess(context, index);
            if (leftResult.NextIndex == index)
            {
                return new ExpressionBlockResult(null, null, index);
            }

            var childNodes = new List<ParseNode>();
            ExpressionBlock expression = leftResult.Block;
            childNodes.Add(leftResult.ParseNode);
            
            index = SkipSpace(context, leftResult.NextIndex).NextIndex;

            bool foundAnyOperator = false;

            while (true)
            {
                var operatorResult = GetIdentifier(context, index, true);
                if (operatorResult.NextIndex == index)
                {
                    break;
                }

                var func = context.ReferenceProvider.Get(operatorResult.IdenLower);
                if (!(func is IFsFunction inf) || inf.CallType != CallType.Dual)
                {
                    break;
                }

                foundAnyOperator = true;

                childNodes.Add(operatorResult.ParseNode);

                index = SkipSpace(context, operatorResult.NextIndex).NextIndex;

                var rightResult = GetCallAndMemberAccess(context, index);
                if (rightResult.NextIndex == index)
                {
                    context.SyntaxErrors.Add(new SyntaxErrorData(
                        index,
                        0,
                        $"Right side operand expected for {operatorResult.Iden}"
                    ));
                    return new ExpressionBlockResult(null, null, originalIndex);
                }

                childNodes.Add(rightResult.ParseNode);
                index = SkipSpace(context, rightResult.NextIndex).NextIndex;

                var operands = new List<ExpressionBlock> { expression, rightResult.Block };

                while (true)
                {
                    var literalResult = GetLiteralMatch(context, index, "~");
                    if (literalResult.NextIndex == index)
                    {
                        break;
                    }

                    index = SkipSpace(context, literalResult.NextIndex).NextIndex;
                    var moreOperandResult = GetCallAndMemberAccess(context, index);
                    if (moreOperandResult.NextIndex == index)
                    {
                        break;
                    }

                    childNodes.Add(moreOperandResult.ParseNode);
                    index = SkipSpace(context, moreOperandResult.NextIndex).NextIndex;

                    operands.Add(moreOperandResult.Block);
                }

                var callExpr = new FunctionCallExpression
                {
                    Function = new LiteralBlock(func),
                    Parameters = operands.ToArray()
                };

                expression = callExpr;
            }

            if (!foundAnyOperator)
            {
                return leftResult;
            }

            int startPos = childNodes[0].Pos;
            var lastNode = childNodes[childNodes.Count - 1];
            int length = (lastNode.Pos + lastNode.Length) - startPos;
            
            var infixNode = new ParseNode(
                ParseNodeType.InfixExpression,
                startPos,
                length,
                childNodes
            );

            return new ExpressionBlockResult(expression, infixNode, index);
        }
    }
}
