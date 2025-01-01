"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.GetReturnDefinition = GetReturnDefinition;
const FuncScriptParser_Main_1 = require("../FuncScriptParser.Main");
const FuncScriptParser_GetExpression_1 = require("./FuncScriptParser.GetExpression");
const FuncScriptParser_GetLiteralMatch_1 = require("./FuncScriptParser.GetLiteralMatch");
const FuncScriptParser_SkipSpace_1 = require("./FuncScriptParser.SkipSpace");
function GetReturnDefinition(context, index) {
    let parseNode = null;
    let i = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, index, FuncScriptParser_Main_1.KW_RETURN).NextIndex;
    if (i === index) {
        return { ParseNode: null, NextIndex: index };
    }
    const nodeReturn = new FuncScriptParser_Main_1.ParseNode(FuncScriptParser_Main_1.ParseNodeType.KeyWord, index, i - index);
    i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, i).NextIndex;
    const exprResult = (0, FuncScriptParser_GetExpression_1.GetExpression)(context, i);
    if (exprResult.NextIndex === i) {
        context.SyntaxErrors.push(new FuncScriptParser_Main_1.SyntaxErrorData(i, 0, "return expression expected"));
        return { ParseNode: null, NextIndex: index };
    }
    i = exprResult.NextIndex;
    parseNode = new FuncScriptParser_Main_1.ParseNode(FuncScriptParser_Main_1.ParseNodeType.ReturnExpression, index, i - index, [nodeReturn, exprResult.ParseNode]);
    return { ParseNode: parseNode, NextIndex: i };
}
