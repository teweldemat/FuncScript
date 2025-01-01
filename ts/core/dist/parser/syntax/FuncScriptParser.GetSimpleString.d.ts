import { ParseContext, ParseNode, ParseResult } from "../FuncScriptParser.Main";
export interface GetSimpleStringResult extends ParseResult {
    Str: string | null;
    ParseNode: ParseNode | null;
    NextIndex: number;
}
export declare function GetSimpleString(context: ParseContext, index: number): GetSimpleStringResult;
