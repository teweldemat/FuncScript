"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.GetSpaceLessString = GetSpaceLessString;
const FuncScriptParser_Main_1 = require("../FuncScriptParser.Main");
const FuncScriptParser_Utils_1 = require("../FuncScriptParser.Utils");
function GetSpaceLessString(context, index) {
    if (index >= context.Expression.length) {
        return { ParseNode: null, NextIndex: index };
    }
    let i = index;
    // Assuming a placeholder for isCharWhiteSpace as it's not to be implemented
    if (i >= context.Expression.length || (0, FuncScriptParser_Utils_1.isCharWhiteSpace)(context.Expression[i])) {
        return { ParseNode: null, NextIndex: index };
    }
    i++;
    while (i < context.Expression.length && !(0, FuncScriptParser_Utils_1.isCharWhiteSpace)(context.Expression[i])) {
        i++;
    }
    const text = context.Expression.substring(index, i);
    const parseNode = new FuncScriptParser_Main_1.ParseNode(FuncScriptParser_Main_1.ParseNodeType.Identifier, index, i - index);
    return { ParseNode: parseNode, NextIndex: i };
}
