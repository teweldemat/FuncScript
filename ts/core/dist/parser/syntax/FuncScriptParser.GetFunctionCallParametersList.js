"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.GetFunctionCallParametersList = GetFunctionCallParametersList;
const FuncScriptParser_Main_1 = require("../FuncScriptParser.Main");
const FuncScriptParser_GetExpression_1 = require("./FuncScriptParser.GetExpression");
const FuncScriptParser_GetLiteralMatch_1 = require("./FuncScriptParser.GetLiteralMatch");
const FuncScriptParser_SkipSpace_1 = require("./FuncScriptParser.SkipSpace");
function GetFunctionCallParametersList(context, index) {
    const result = getFunctionCallParametersList(context, "(", ")", index);
    if (result.NextIndex === index) {
        return getFunctionCallParametersList(context, "[", "]", index);
    }
    return result;
}
function getFunctionCallParametersList(context, openBrace, closeBrace, index) {
    let parseNode = null;
    let prog = null;
    let i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, index).NextIndex;
    let i2 = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, i, openBrace).NextIndex;
    if (i === i2) {
        return { ParseNode: null, NextIndex: index };
    }
    i = i2;
    const parseNodes = [];
    i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, i).NextIndex;
    let exprResult = (0, FuncScriptParser_GetExpression_1.GetExpression)(context, i);
    if (exprResult.NextIndex > i) {
        i = exprResult.NextIndex;
        parseNodes.push(exprResult.ParseNode);
        do {
            i2 = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, i).NextIndex;
            if (i2 >= context.Expression.length || context.Expression[i2++] !== ',') {
                break;
            }
            i = i2;
            i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, i).NextIndex;
            exprResult = (0, FuncScriptParser_GetExpression_1.GetExpression)(context, i);
            if (exprResult.NextIndex === i) {
                context.SyntaxErrors.push(new FuncScriptParser_Main_1.SyntaxErrorData(i, 0, "Parameter for call expected"));
                return { ParseNode: null, NextIndex: index };
            }
            i = exprResult.NextIndex;
            parseNodes.push(exprResult.ParseNode);
        } while (true);
    }
    i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, i).NextIndex;
    i2 = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, i, closeBrace).NextIndex;
    if (i2 === i) {
        context.SyntaxErrors.push(new FuncScriptParser_Main_1.SyntaxErrorData(i, 0, `'${closeBrace}' expected`));
        return { ParseNode: null, NextIndex: index };
    }
    i = i2;
    parseNode = new FuncScriptParser_Main_1.ParseNode(FuncScriptParser_Main_1.ParseNodeType.FunctionParameterList, index, i - index, parseNodes);
    return { ParseNode: parseNode, NextIndex: i };
}
