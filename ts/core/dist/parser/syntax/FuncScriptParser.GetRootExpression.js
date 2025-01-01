"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.GetRootExpression = GetRootExpression;
const FuncScriptParser_GetExpression_1 = require("./FuncScriptParser.GetExpression");
function GetRootExpression(context, index) {
    const thisErrors = [];
    const result = (0, FuncScriptParser_GetExpression_1.GetExpression)(context, index);
    if (result.NextIndex > index) {
        context.SyntaxErrors.push(...thisErrors);
        return result;
    }
    return { ParseNode: null, NextIndex: index };
}
