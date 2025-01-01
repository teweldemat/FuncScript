using funcscript.block;
using funcscript.model;
using System.Collections.Generic;

namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        static ExpressionBlockResult GetInfixFunctionCall(ParseContext context, int index)
        {
            // Keep track of our entry point in case we need to "reject"
            // when we don't find any infix operator
            int originalIndex = index;

            // 1) Parse the left-most operand
            var leftResult = GetCallAndMemberAccess(context, index);
            if (leftResult.NextIndex == index)
            {
                // No operand at all, fail immediately
                return new ExpressionBlockResult(null, null, index);
            }

            // We'll build up child parse nodes and the expression by folding left-to-right
            var childNodes = new List<ParseNode>();
            ExpressionBlock expression = leftResult.Block;
            childNodes.Add(leftResult.ParseNode);
            
            // Move to where the left operand parsing ended
            index = SkipSpace(context, leftResult.NextIndex).NextIndex;

            bool foundAnyOperator = false;

            // 2) In a loop, parse "operator + operand(s)" chains
            while (true)
            {
                // Try to get an identifier as a potential "infix operator"
                var operatorResult = GetIdentifier(context, index,true);
                if (operatorResult.NextIndex == index)
                {
                    // No operator found; break out
                    break;
                }

                // Check if identifier indeed represents a dual-call (infix) function
                var func = context.ReferenceProvider.Get(operatorResult.IdenLower);
                if (!(func is IFsFunction inf) || inf.CallType != CallType.Dual)
                {
                    // Not a valid infix operator; break out
                    break;
                }

                // We did find an infix operator
                foundAnyOperator = true;

                // Append the operator node
                childNodes.Add(operatorResult.ParseNode);

                // Advance index
                index = SkipSpace(context, operatorResult.NextIndex).NextIndex;

                // Next must be at least one operand
                var rightResult = GetCallAndMemberAccess(context, index);
                if (rightResult.NextIndex == index)
                {
                    // We expected something on the right of the operator
                    context.SyntaxErrors.Add(new SyntaxErrorData(
                        index,
                        0,
                        $"Right side operand expected for {operatorResult.Iden}"
                    ));
                    return new ExpressionBlockResult(null, null, originalIndex);
                }

                // Collect the new operand parse node
                childNodes.Add(rightResult.ParseNode);
                index = SkipSpace(context, rightResult.NextIndex).NextIndex;

                // Gather operands for the current operator call
                var operands = new List<ExpressionBlock> { expression, rightResult.Block };

                // Check if this operator can also gather extra operands via '~'
                while (true)
                {
                    var literalResult = GetLiteralMatch(context, index,  "~" );
                    if (literalResult.NextIndex == index)
                    {
                        // No "~" found; stop collecting extra operands
                        break;
                    }

                    // We got a tilde, skip and parse next operand
                    index = SkipSpace(context, literalResult.NextIndex).NextIndex;
                    var moreOperandResult = GetCallAndMemberAccess(context, index);
                    if (moreOperandResult.NextIndex == index)
                    {
                        // No operand after "~", break
                        break;
                    }

                    // Add the extra operand parse node
                    childNodes.Add(moreOperandResult.ParseNode);
                    index = SkipSpace(context, moreOperandResult.NextIndex).NextIndex;

                    // Add the extra operand to the operator's argument list
                    operands.Add(moreOperandResult.Block);
                }

                // Build a new function call for this operator:
                // operator(expression, rightOperand, [possible extra operands...])
                var callExpr = new FunctionCallExpression
                {
                    Function = new LiteralBlock(func),
                    Parameters = operands.ToArray()
                };
                callExpr.SetContext(context.ReferenceProvider);

                // That becomes our new "running" expression
                expression = callExpr;
            }

            // If we found no operator at all, reject (return null)
            if (!foundAnyOperator)
            {
                return leftResult;
            }

            // Build a single parse node to represent the entire chained infix expression
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