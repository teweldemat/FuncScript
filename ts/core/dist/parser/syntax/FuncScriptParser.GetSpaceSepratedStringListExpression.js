"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.GetSpaceSeparatedStringListExpression = GetSpaceSeparatedStringListExpression;
const FuncScriptParser_Main_1 = require("../FuncScriptParser.Main");
const FuncScriptParser_GetLiteralMatch_1 = require("./FuncScriptParser.GetLiteralMatch");
const FuncScriptParser_GetSimpleString_1 = require("./FuncScriptParser.GetSimpleString");
const FuncScriptParser_GetSpaceLessString_1 = require("./FuncScriptParser.GetSpaceLessString");
const FuncScriptParser_SkipSpace_1 = require("./FuncScriptParser.SkipSpace");
function GetSpaceSeparatedStringListExpression(context, index) {
    let i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, index).NextIndex;
    const nodeListItems = [];
    let otherNode;
    const firstResult = (0, FuncScriptParser_GetSimpleString_1.GetSimpleString)(context, i);
    let firstNode = firstResult.ParseNode;
    let i2 = firstResult.NextIndex;
    if (i2 === i) {
        const alternativeResult = (0, FuncScriptParser_GetSpaceLessString_1.GetSpaceLessString)(context, i);
        firstNode = alternativeResult.ParseNode;
        i2 = alternativeResult.NextIndex;
    }
    if (i2 > i) {
        nodeListItems.push(firstNode);
        i = i2;
        do {
            i2 = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, i, " ").NextIndex;
            if (i2 === i)
                break;
            i = i2;
            i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, i).NextIndex;
            const stringResult = (0, FuncScriptParser_GetSimpleString_1.GetSimpleString)(context, i);
            i2 = stringResult.NextIndex;
            if (i2 === i) {
                const spaceLessResult = (0, FuncScriptParser_GetSpaceLessString_1.GetSpaceLessString)(context, i);
                otherNode = spaceLessResult.ParseNode;
                i2 = spaceLessResult.NextIndex;
            }
            else
                otherNode = stringResult.ParseNode;
            if (i2 === i)
                break;
            nodeListItems.push(otherNode);
            i = i2;
        } while (true);
    }
    const parseNode = new FuncScriptParser_Main_1.ParseNode(FuncScriptParser_Main_1.ParseNodeType.List, index, i - index, nodeListItems);
    return { ParseNode: parseNode, NextIndex: i };
}
