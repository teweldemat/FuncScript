import { ParseContext, ParseNode } from "../FuncScriptParser.Main";
export declare function GetNumber(context: ParseContext, index: number): {
    ParseNode: ParseNode | null;
    NextIndex: number;
};
