"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.GetCaseExpression = GetCaseExpression;
const FuncScriptParser_Main_1 = require("../FuncScriptParser.Main");
const FuncScriptParser_GetExpression_1 = require("./FuncScriptParser.GetExpression");
const FuncScriptParser_GetLiteralMatch_1 = require("./FuncScriptParser.GetLiteralMatch");
const FuncScriptParser_SkipSpace_1 = require("./FuncScriptParser.SkipSpace");
function GetCaseExpression(context, index) {
    let parseNode = null;
    let i = index;
    let literalMatchResult = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, i, FuncScriptParser_Main_1.KW_CASE);
    if (literalMatchResult.NextIndex === i) {
        context.SyntaxErrors.push(new FuncScriptParser_Main_1.SyntaxErrorData(i, 1, "Case keyword expected"));
        return { ParseNode: null, NextIndex: index };
    }
    i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, literalMatchResult.NextIndex).NextIndex;
    const childNodes = [];
    do {
        let expressionResult = (0, FuncScriptParser_GetExpression_1.GetExpression)(context, i);
        if (expressionResult.NextIndex === i) {
            context.SyntaxErrors.push(new FuncScriptParser_Main_1.SyntaxErrorData(i, 1, "Case condition expected"));
            return { ParseNode: null, NextIndex: index };
        }
        childNodes.push(expressionResult.ParseNode);
        i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, expressionResult.NextIndex).NextIndex;
        literalMatchResult = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, i, ":");
        if (literalMatchResult.NextIndex === i) {
            break;
        }
        i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, literalMatchResult.NextIndex).NextIndex;
        let valueExpressionResult = (0, FuncScriptParser_GetExpression_1.GetExpression)(context, i);
        if (valueExpressionResult.NextIndex === i) {
            context.SyntaxErrors.push(new FuncScriptParser_Main_1.SyntaxErrorData(i, 1, "Case value expected"));
            return { ParseNode: null, NextIndex: index };
        }
        childNodes.push(valueExpressionResult.ParseNode);
        i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, valueExpressionResult.NextIndex).NextIndex;
    } while (true);
    parseNode = new FuncScriptParser_Main_1.ParseNode(FuncScriptParser_Main_1.ParseNodeType.Case, index, i - index);
    parseNode.Children = childNodes;
    return { ParseNode: parseNode, NextIndex: i };
}
