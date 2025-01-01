"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.Parse = Parse;
const FuncScriptParser_GetRootExpression_1 = require("./FuncScriptParser.GetRootExpression");
function Parse(context) {
    const rootExpressionResult = (0, FuncScriptParser_GetRootExpression_1.GetRootExpression)(context, 0);
    return rootExpressionResult;
}
