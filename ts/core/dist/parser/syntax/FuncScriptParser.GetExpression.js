"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.GetExpression = GetExpression;
const FuncScriptParser_GetInfixExpression_1 = require("./FuncScriptParser.GetInfixExpression");
function GetExpression(context, index) {
    const result = (0, FuncScriptParser_GetInfixExpression_1.GetInfixExpression)(context, index);
    if (result.NextIndex > index) {
        return result;
    }
    return { ParseNode: null, NextIndex: index };
}
