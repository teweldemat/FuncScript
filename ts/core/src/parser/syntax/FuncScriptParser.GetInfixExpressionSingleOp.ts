import { ParseContext, ParseNode, ParseNodeType, ParseResult, s_operatorSymbols, SyntaxErrorData } from "../FuncScriptParser.Main";
import { GetCallAndMemberAccess } from "./FuncScriptParser.GetCallAndMemberAccess";
import { GetLiteralMatch } from "./FuncScriptParser.GetLiteralMatch";
import { GetOperator } from "./FuncScriptParser.GetOperator";
import { SkipSpace } from "./FuncScriptParser.SkipSpace";

export function GetInfixExpressionSingleOp(context: ParseContext, level: number, candidates: string[], index: number)
    :ParseResult
{
    let parseNode: ParseNode | null = null;
    let i = index;

    while (true) {
        let i2: number;
        let operatorNode: ParseNode | null = null;
        let symbol: string | null = null;

        if (parseNode === null) {
            let result;
            if (level === 0) {
                result = GetCallAndMemberAccess(context, i);
            } else {
                result = GetInfixExpressionSingleOp(context, level - 1, s_operatorSymbols[level - 1], i);
            }

            parseNode = result.ParseNode;
            i2 = result.NextIndex;

            if (i2 === i) return {  ParseNode: null, NextIndex: i };

            i = SkipSpace(context, i2).NextIndex;
            continue;
        }

        const indexBeforeOperator = i;
        const operatorResult = GetOperator(context, candidates, i);
        symbol = operatorResult.MatchedOp;
        operatorNode = operatorResult.ParseNode;
        i2 = operatorResult.NextIndex;

        if (i2 === i) break;

        i = SkipSpace(context, i2).NextIndex;

        const operandNodes: ParseNode[] = [];
        operandNodes.push(parseNode);

        while (true) {
            let nextOperandNode: ParseNode | null = null;
            let nextOperandResult;
            if (level === 0) {
                nextOperandResult = GetCallAndMemberAccess(context, i);
            } else {
                nextOperandResult = GetInfixExpressionSingleOp(context, level - 1, s_operatorSymbols[level - 1], i);
            }

            nextOperandNode = nextOperandResult.ParseNode;
            i2 = nextOperandResult.NextIndex;

            if (i2 === i) return { ParseNode: null, NextIndex: indexBeforeOperator };

            operandNodes.push(nextOperandNode!);
            i = SkipSpace(context, i2).NextIndex;

            i2 = GetLiteralMatch(context, i, symbol!).NextIndex;
            if (i2 === i) break;
            i = SkipSpace(context, i2).NextIndex;
        }

        if (operandNodes.length > 1 && symbol === "|") {
            if (operandNodes.length > 2) {
                context.SyntaxErrors.push(new SyntaxErrorData(i, 0, "Only two parameters expected for | "));
                return { ParseNode: null, NextIndex: i };
            }

            parseNode = new ParseNode(ParseNodeType.InfixExpression, parseNode.Pos,
                operandNodes[operandNodes.length - 1].Pos + operandNodes[operandNodes.length - 1].Length - parseNode.Length);

        }
        else {
            parseNode = new ParseNode(ParseNodeType.InfixExpression, parseNode.Pos,
                operandNodes[operandNodes.length - 1].Pos + operandNodes[operandNodes.length - 1].Length - parseNode.Length);
        }
    }
    return {ParseNode: parseNode, NextIndex: i };
}