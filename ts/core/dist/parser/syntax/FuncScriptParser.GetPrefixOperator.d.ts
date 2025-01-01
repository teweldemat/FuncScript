import { ParseContext, ParseNode } from "../FuncScriptParser.Main";
export declare function GetPrefixOperator(context: ParseContext, index: number): {
    ParseNode: null;
    NextIndex: number;
} | {
    ParseNode: ParseNode;
    NextIndex: number;
};
