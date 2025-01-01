import { ParseContext, ParseNode } from "../FuncScriptParser.Main";
export declare function GetKvcExpression(context: ParseContext, nakdeMode: boolean, index: number): {
    ParseNode: null;
    NextIndex: number;
} | {
    ParseNode: ParseNode;
    NextIndex: number;
};
