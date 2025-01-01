import { ParseContext, ParseNode, ParseNodeType, ParseResult } from "../FuncScriptParser.Main";
import { GetLiteralMatch } from "./FuncScriptParser.GetLiteralMatch";

interface GetOperatorResult extends ParseResult {
    MatchedOp: string | null;
}

export function GetOperator(context: ParseContext, candidates: string[], index: number): GetOperatorResult {
    for (const op of candidates) {
        const literalMatchResult = GetLiteralMatch(context, index, op);
        const i = literalMatchResult.NextIndex;
        if (i <= index) continue;

        const parseNode = new ParseNode(ParseNodeType.Operator, index, i - index);
        return {
            MatchedOp: op,
            ParseNode: parseNode,
            NextIndex: i
        };
    }

    return {
        MatchedOp: null,
        ParseNode: null,
        NextIndex: index
    };
}