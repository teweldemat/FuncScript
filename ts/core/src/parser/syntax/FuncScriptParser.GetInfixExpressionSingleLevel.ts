import { ParseContext, ParseNode, ParseNodeType, s_operatorSymbols, SyntaxErrorData } from "../FuncScriptParser.Main";
import { GetInfixFunctionCall } from "./FuncScriptParser.GetInfixFunctionCall";
import { GetOperator } from "./FuncScriptParser.GetOperator";
import { SkipSpace } from "./FuncScriptParser.SkipSpace";

export function GetInfixExpressionSingleLevel(context: ParseContext, level: number, candidates: string[], index: number): { ParseNode: ParseNode | null, NextIndex: number } {
    let parseNode: ParseNode | null = null;
    let i = index;

    while (true) {
        let i2: number;
        let symbol: string | null = null;
        let operatorNode: ParseNode | null = null;

        if (parseNode === null) {
            let result;
            if (level === 0) {
                result = GetInfixFunctionCall(context, i);
            } else {
                result = GetInfixExpressionSingleLevel(context, level - 1, s_operatorSymbols[level - 1], i);
            }

            parseNode = result.ParseNode;
            i2 = result.NextIndex;

            if (i2 === i) {
                return { ParseNode: parseNode, NextIndex: i };
            }

            i = SkipSpace(context, i2).NextIndex;
            continue;
        }

        const indexBeforeOperator = i;
        const operatorResult = GetOperator(context, candidates, i);
        symbol = operatorResult.MatchedOp;
        operatorNode = operatorResult.ParseNode;
        i2 = operatorResult.NextIndex;

        if (i2 === i) {
            break;
        }

        i = SkipSpace(context, i2).NextIndex;

        const infixComponentNodes: ParseNode[] = [parseNode];

        while (true) {
            let nextOperandNode: ParseNode | null;
            if (level === 0) {
                const result = GetInfixFunctionCall(context, i);
                nextOperandNode = result.ParseNode;
                i2 = result.NextIndex;
            } else {
                const result = GetInfixExpressionSingleLevel(context, level - 1, s_operatorSymbols[level - 1], i);
                nextOperandNode = result.ParseNode;
                i2 = result.NextIndex;
            }

            if (i2 === i) {
                return { ParseNode: parseNode, NextIndex: indexBeforeOperator };
            }
            infixComponentNodes.push(operatorNode!);
            infixComponentNodes.push(nextOperandNode!);
            i = SkipSpace(context, i2).NextIndex;
            const nextOperatorResult = GetOperator(context, [symbol!], i);
            i2 = nextOperatorResult.NextIndex;
            if (i2 === i) {
                break;
            }
            operatorNode=nextOperatorResult.ParseNode
            i = SkipSpace(context, i2).NextIndex;
        }

        if (infixComponentNodes.length > 1) {
            parseNode = new ParseNode(
                ParseNodeType.InfixExpression,
                parseNode.Pos,
                infixComponentNodes[infixComponentNodes.length - 1].Pos + infixComponentNodes[infixComponentNodes.length - 1].Length - parseNode.Pos,
                infixComponentNodes
            );
        }
    }

    return { ParseNode: parseNode, NextIndex: i };
}