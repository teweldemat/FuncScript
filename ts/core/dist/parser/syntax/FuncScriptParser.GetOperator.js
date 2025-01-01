"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.GetOperator = GetOperator;
const FuncScriptParser_Main_1 = require("../FuncScriptParser.Main");
const FuncScriptParser_GetLiteralMatch_1 = require("./FuncScriptParser.GetLiteralMatch");
function GetOperator(context, candidates, index) {
    for (const op of candidates) {
        const literalMatchResult = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, index, op);
        const i = literalMatchResult.NextIndex;
        if (i <= index)
            continue;
        const parseNode = new FuncScriptParser_Main_1.ParseNode(FuncScriptParser_Main_1.ParseNodeType.Operator, index, i - index);
        return { ParseNode: parseNode, NextIndex: i, MatchedOp: op };
    }
    return { ParseNode: null, NextIndex: index, MatchedOp: null };
}
