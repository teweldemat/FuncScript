"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.GetFSTemplate = GetFSTemplate;
const FuncScriptParser_Main_1 = require("../FuncScriptParser.Main");
const FuncScriptParser_GetExpression_1 = require("./FuncScriptParser.GetExpression");
const FuncScriptParser_GetLiteralMatch_1 = require("./FuncScriptParser.GetLiteralMatch");
const FuncScriptParser_SkipSpace_1 = require("./FuncScriptParser.SkipSpace");
function GetFSTemplate(context, index) {
    let parseNode = null;
    let i = index;
    let i2;
    let lastIndex = i;
    let nodeParts = [];
    while (true) {
        i2 = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, i, "$${").NextIndex;
        if (i2 > i) {
            i = i2;
        }
        i2 = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, i, "${").NextIndex;
        if (i2 > i) {
            i = i2;
            i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, i).NextIndex;
            const resultExpr = (0, FuncScriptParser_GetExpression_1.GetExpression)(context, i);
            const nodeExpr = resultExpr.ParseNode;
            i2 = resultExpr.NextIndex;
            if (i2 === i) {
                context.SyntaxErrors.push(new FuncScriptParser_Main_1.SyntaxErrorData(i, 0, "expression expected"));
                return { ParseNode: null, NextIndex: index };
            }
            i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, i).NextIndex;
            nodeParts.push(nodeExpr);
            i = i2;
            i2 = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, i, "}").NextIndex;
            if (i2 === i) {
                context.SyntaxErrors.push(new FuncScriptParser_Main_1.SyntaxErrorData(i, 0, "'}' expected"));
                return { ParseNode: null, NextIndex: index };
            }
            i = i2;
            lastIndex = i;
            if (i < context.Expression.length)
                continue;
            else
                break;
        }
        i++;
        if (i === context.Expression.length)
            break;
    }
    if (nodeParts.length === 0) {
        parseNode = new FuncScriptParser_Main_1.ParseNode(FuncScriptParser_Main_1.ParseNodeType.LiteralString, index, i - index);
    }
    else if (nodeParts.length === 1) {
        parseNode = nodeParts[0];
    }
    else {
        parseNode = new FuncScriptParser_Main_1.ParseNode(FuncScriptParser_Main_1.ParseNodeType.StringTemplate, index, i - index, nodeParts);
    }
    return { ParseNode: parseNode, NextIndex: i };
}
