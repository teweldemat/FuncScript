import { ParseContext, ParseResult } from "../FuncScriptParser.Main";
export interface GetIdentifierResult extends ParseResult {
    Iden: string | null;
    IdenLower: string | null;
    ParentRef: boolean;
}
export declare function GetIdentifier(context: ParseContext, index: number, supportParentRef: boolean): GetIdentifierResult;
