import { ParseContext, ParseResult } from "../FuncScriptParser.Main";
export interface GetOperatorResult extends ParseResult {
    MatchedOp: string | null;
}
export declare function GetOperator(context: ParseContext, candidates: string[], index: number): GetOperatorResult;
