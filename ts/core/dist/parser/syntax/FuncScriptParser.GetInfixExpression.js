"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.GetInfixExpression = GetInfixExpression;
const FuncScriptParser_Main_1 = require("../FuncScriptParser.Main");
const FuncScriptParser_GetInfixExpressionSingleLevel_1 = require("./FuncScriptParser.GetInfixExpressionSingleLevel");
function GetInfixExpression(context, index) {
    const result = (0, FuncScriptParser_GetInfixExpressionSingleLevel_1.GetInfixExpressionSingleLevel)(context, FuncScriptParser_Main_1.s_operatorSymols.length - 1, FuncScriptParser_Main_1.s_operatorSymols[FuncScriptParser_Main_1.s_operatorSymols.length - 1], index);
    return result;
}
