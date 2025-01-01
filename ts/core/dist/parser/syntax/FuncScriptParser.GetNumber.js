"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.GetNumber = GetNumber;
const FuncScriptParser_Main_1 = require("../FuncScriptParser.Main");
const FuncScriptParser_GetInt_1 = require("./FuncScriptParser.GetInt");
const FuncScriptParser_GetLiteralMatch_1 = require("./FuncScriptParser.GetLiteralMatch");
function GetNumber(context, index) {
    let parseNode = null;
    let hasDecimal = false;
    let hasExp = false;
    let hasLong = false;
    let i = index;
    const intResult = (0, FuncScriptParser_GetInt_1.GetInt)(context, true, i);
    let { ParseNode: nodeDigits, NextIndex: i2 } = intResult;
    if (i2 === i) {
        return { ParseNode: null, NextIndex: index };
    }
    i = i2;
    i2 = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, i, ".").NextIndex;
    if (i2 > i) {
        hasDecimal = true;
    }
    i = i2;
    if (hasDecimal) {
        const decimalResult = (0, FuncScriptParser_GetInt_1.GetInt)(context, false, i);
        i = decimalResult.NextIndex;
    }
    i2 = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, i, "E").NextIndex;
    if (i2 > i) {
        hasExp = true;
    }
    i = i2;
    let expDigits = null;
    let nodeExpDigits;
    if (hasExp) {
        const expResult = (0, FuncScriptParser_GetInt_1.GetInt)(context, true, i);
        nodeExpDigits = expResult.ParseNode;
        i = expResult.NextIndex;
    }
    if (!hasDecimal) {
        i2 = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, i, "l").NextIndex;
        if (i2 > i) {
            hasLong = true;
        }
        i = i2;
    }
    if (hasDecimal) {
        parseNode = new FuncScriptParser_Main_1.ParseNode(FuncScriptParser_Main_1.ParseNodeType.LiteralDouble, index, i - index);
        return { ParseNode: parseNode, NextIndex: i };
    }
    let longVal;
    if (hasLong) {
        parseNode = new FuncScriptParser_Main_1.ParseNode(FuncScriptParser_Main_1.ParseNodeType.LiteralLong, index, i - index);
        return { ParseNode: parseNode, NextIndex: i };
    }
    parseNode = new FuncScriptParser_Main_1.ParseNode(FuncScriptParser_Main_1.ParseNodeType.LiteralInteger, index, i - index);
    return { ParseNode: parseNode, NextIndex: i };
}
