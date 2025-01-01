import { ParseContext } from "../FuncScriptParser.Main";
export declare function GetInfixExpression(context: ParseContext, index: number): {
    ParseNode: import("../FuncScriptParser.Main").ParseNode | null;
    NextIndex: number;
};
