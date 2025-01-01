"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.GetKeyWordLiteral = GetKeyWordLiteral;
const FuncScriptParser_Main_1 = require("../FuncScriptParser.Main");
const FuncScriptParser_GetLiteralMatch_1 = require("./FuncScriptParser.GetLiteralMatch");
function GetKeyWordLiteral(context, index) {
    let parseNode = null;
    let i = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, index, "null").NextIndex;
    let literal;
    if (i > index) {
        literal = null;
    }
    else if ((i = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, index, "true").NextIndex) > index) {
        literal = true;
    }
    else if ((i = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, index, "false").NextIndex) > index) {
        literal = false;
    }
    else {
        return { ParseNode: null, NextIndex: index };
    }
    parseNode = new FuncScriptParser_Main_1.ParseNode(FuncScriptParser_Main_1.ParseNodeType.KeyWord, index, i - index);
    return { ParseNode: parseNode, NextIndex: i };
}
