"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.GetExpInParenthesis = GetExpInParenthesis;
const FuncScriptParser_Main_1 = require("../FuncScriptParser.Main");
const FuncScriptParser_GetExpression_1 = require("./FuncScriptParser.GetExpression");
const FuncScriptParser_GetLiteralMatch_1 = require("./FuncScriptParser.GetLiteralMatch");
const FuncScriptParser_SkipSpace_1 = require("./FuncScriptParser.SkipSpace");
function GetExpInParenthesis(context, index) {
    let parseNode = null;
    let i = index;
    i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, i).NextIndex;
    let i2 = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, i, "(").NextIndex;
    if (i === i2) {
        return { ParseNode: parseNode, NextIndex: index };
    }
    i = i2;
    i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, i).NextIndex;
    const expressionResult = (0, FuncScriptParser_GetExpression_1.GetExpression)(context, i);
    const nodeExpression = expressionResult.ParseNode;
    if (expressionResult.NextIndex > i) {
        i = expressionResult.NextIndex;
    }
    i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, i).NextIndex;
    i2 = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, i, ")").NextIndex;
    if (i === i2) {
        context.SyntaxErrors.push(new FuncScriptParser_Main_1.SyntaxErrorData(i, 0, "')' expected"));
        return { ParseNode: parseNode, NextIndex: index };
    }
    i = i2;
    parseNode = new FuncScriptParser_Main_1.ParseNode(FuncScriptParser_Main_1.ParseNodeType.ExpressionInBrace, index, i - index, [nodeExpression]);
    return { ParseNode: parseNode, NextIndex: i };
}
