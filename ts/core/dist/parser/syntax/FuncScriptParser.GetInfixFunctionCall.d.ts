import { ParseContext, ParseNode } from "../FuncScriptParser.Main";
export declare function GetInfixFunctionCall(context: ParseContext, index: number): {
    ParseNode: ParseNode | null;
    NextIndex: number;
};
