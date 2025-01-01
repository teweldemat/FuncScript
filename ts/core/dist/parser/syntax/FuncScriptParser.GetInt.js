"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.GetInt = GetInt;
const FuncScriptParser_Main_1 = require("../FuncScriptParser.Main");
const FuncScriptParser_GetLiteralMatch_1 = require("./FuncScriptParser.GetLiteralMatch");
function GetInt(context, allowNegative, index) {
    let parseNode = null;
    let i = index;
    if (allowNegative) {
        i = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, i, "-").NextIndex;
    }
    let i2 = i;
    while (i2 < context.Expression.length && /\d/.test(context.Expression[i2])) {
        i2++;
    }
    if (i === i2) {
        return { ParseNode: null, NextIndex: i };
    }
    i = i2;
    const intVal = context.Expression.substring(index, i);
    parseNode = new FuncScriptParser_Main_1.ParseNode(FuncScriptParser_Main_1.ParseNodeType.LiteralInteger, index, i - index);
    return { ParseNode: parseNode, NextIndex: i };
}
