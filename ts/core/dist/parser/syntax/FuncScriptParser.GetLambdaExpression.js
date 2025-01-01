"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.GetLambdaExpression = GetLambdaExpression;
const FuncScriptParser_Main_1 = require("../FuncScriptParser.Main");
const FuncScriptParser_GetIdentifierList_1 = require("./FuncScriptParser.GetIdentifierList");
const FuncScriptParser_SkipSpace_1 = require("./FuncScriptParser.SkipSpace");
const FuncScriptParser_GetLiteralMatch_1 = require("./FuncScriptParser.GetLiteralMatch");
const FuncScriptParser_GetExpression_1 = require("./FuncScriptParser.GetExpression");
function GetLambdaExpression(context, index) {
    let parseNode = null;
    const { ParseNode: nodesParams, NextIndex: iInitial } = (0, FuncScriptParser_GetIdentifierList_1.GetIdentifierList)(context, index);
    let i = iInitial;
    if (i === index) {
        return { ParseNode: null, NextIndex: index };
    }
    i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, i).NextIndex;
    if (i >= context.Expression.length - 1) {
        return { ParseNode: null, NextIndex: index };
    }
    let i2 = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, i, "=>").NextIndex;
    if (i2 === i) {
        context.SyntaxErrors.push(new FuncScriptParser_Main_1.SyntaxErrorData(i, 0, "'=>' expected"));
        return { ParseNode: null, NextIndex: index };
    }
    i += 2;
    i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, i).NextIndex;
    const { ParseNode: nodeDefination, NextIndex: iExpression } = (0, FuncScriptParser_GetExpression_1.GetExpression)(context, i);
    if (iExpression === i) {
        context.SyntaxErrors.push(new FuncScriptParser_Main_1.SyntaxErrorData(i, 0, "definition of lambda expression expected"));
        return { ParseNode: null, NextIndex: index };
    }
    i = iExpression;
    parseNode = new FuncScriptParser_Main_1.ParseNode(FuncScriptParser_Main_1.ParseNodeType.LambdaExpression, index, i - index, [nodesParams, nodeDefination]);
    return { ParseNode: parseNode, NextIndex: i };
}
