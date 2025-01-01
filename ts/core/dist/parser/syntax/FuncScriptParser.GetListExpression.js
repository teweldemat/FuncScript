"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.GetListExpression = void 0;
const FuncScriptParser_Main_1 = require("../FuncScriptParser.Main");
const FuncScriptParser_GetExpression_1 = require("./FuncScriptParser.GetExpression");
const FuncScriptParser_GetLiteralMatch_1 = require("./FuncScriptParser.GetLiteralMatch");
const FuncScriptParser_SkipSpace_1 = require("./FuncScriptParser.SkipSpace");
const GetListExpression = (context, index) => {
    let i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, index).NextIndex;
    let i2 = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, i, "[").NextIndex;
    if (i2 === i)
        return { ParseNode: null, NextIndex: index };
    let tokenStart = i2;
    i = i2;
    let nodeListItems = [];
    i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, i).NextIndex;
    let expressionResult = (0, FuncScriptParser_GetExpression_1.GetExpression)(context, i);
    if (expressionResult.NextIndex > i) {
        nodeListItems.push(expressionResult.ParseNode);
        i = expressionResult.NextIndex;
        do {
            i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, i).NextIndex;
            i2 = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, i, ",").NextIndex;
            if (i2 === i)
                break;
            i = i2;
            i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, i).NextIndex;
            expressionResult = (0, FuncScriptParser_GetExpression_1.GetExpression)(context, i);
            if (expressionResult.NextIndex === i)
                break;
            nodeListItems.push(expressionResult.ParseNode);
            i = expressionResult.NextIndex;
        } while (true);
    }
    i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, i).NextIndex;
    i2 = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, i, "]").NextIndex;
    if (i2 === i) {
        context.SyntaxErrors.push(new FuncScriptParser_Main_1.SyntaxErrorData(i, 0, "']' expected"));
        return { ParseNode: null, NextIndex: index };
    }
    i = i2;
    let parseNode = new FuncScriptParser_Main_1.ParseNode(FuncScriptParser_Main_1.ParseNodeType.List, index, i - tokenStart, nodeListItems);
    return { ParseNode: parseNode, NextIndex: i };
};
exports.GetListExpression = GetListExpression;
