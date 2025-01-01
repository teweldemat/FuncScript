"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.GetSwitchExpression = GetSwitchExpression;
const FuncScriptParser_Main_1 = require("../FuncScriptParser.Main");
const FuncScriptParser_GetExpression_1 = require("./FuncScriptParser.GetExpression");
const FuncScriptParser_GetLiteralMatch_1 = require("./FuncScriptParser.GetLiteralMatch");
const FuncScriptParser_SkipSpace_1 = require("./FuncScriptParser.SkipSpace");
function GetSwitchExpression(context, index) {
    let parseNode = null;
    let i = index;
    let i2 = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, i, FuncScriptParser_Main_1.KW_SWITCH).NextIndex;
    if (i2 === i) {
        return { ParseNode: null, NextIndex: index };
    }
    i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, i2).NextIndex;
    const childNodes = [];
    const expressionResult = (0, FuncScriptParser_GetExpression_1.GetExpression)(context, i);
    if (expressionResult.NextIndex === i) {
        context.SyntaxErrors.push(new FuncScriptParser_Main_1.SyntaxErrorData(i, 1, "Switch selector expected"));
        return { ParseNode: null, NextIndex: index };
    }
    childNodes.push(expressionResult.ParseNode);
    i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, expressionResult.NextIndex).NextIndex;
    while (true) {
        i2 = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatchMultiple)(context, i, [",", ";"]).NextIndex;
        if (i2 === i) {
            break;
        }
        i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, i2).NextIndex;
        const part1Result = (0, FuncScriptParser_GetExpression_1.GetExpression)(context, i);
        if (part1Result.NextIndex === i) {
            break;
        }
        childNodes.push(part1Result.ParseNode);
        i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, part1Result.NextIndex).NextIndex;
        i2 = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, i, ":").NextIndex;
        if (i2 === i) {
            break;
        }
        i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, i2).NextIndex;
        const part2Result = (0, FuncScriptParser_GetExpression_1.GetExpression)(context, i);
        if (part2Result.NextIndex === i) {
            context.SyntaxErrors.push(new FuncScriptParser_Main_1.SyntaxErrorData(i, 1, "Selector result expected"));
            return { ParseNode: null, NextIndex: index };
        }
        childNodes.push(part2Result.ParseNode);
        i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, part2Result.NextIndex).NextIndex;
    }
    parseNode = new FuncScriptParser_Main_1.ParseNode(FuncScriptParser_Main_1.ParseNodeType.Case, index, i - index);
    parseNode.Children = childNodes;
    return { ParseNode: parseNode, NextIndex: i };
}
