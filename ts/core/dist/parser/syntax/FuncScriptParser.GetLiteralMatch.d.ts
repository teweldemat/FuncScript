import { ParseContext } from "../FuncScriptParser.Main";
export declare function GetLiteralMatch(context: ParseContext, index: number, candidate: string): GetLiteralMatchResult;
export declare function GetLiteralMatchMultiple(context: ParseContext, index: number, candidates: string[]): GetLiteralMatchResult;
export declare function GetLiteralMatchFromString(expression: string, index: number, candidates: string[]): GetLiteralMatchResult;
interface GetLiteralMatchResult {
    Matched: string | null;
    NextIndex: number;
}
export {};
