import { ParseContext, ParseNode } from "../FuncScriptParser.Main";
export declare function GetInfixExpressionSingleLevel(context: ParseContext, level: number, candidates: string[], index: number): {
    ParseNode: ParseNode | null;
    NextIndex: number;
};
