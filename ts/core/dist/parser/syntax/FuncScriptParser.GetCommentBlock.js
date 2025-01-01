"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.GetCommentBlock = GetCommentBlock;
const FuncScriptParser_Main_1 = require("../FuncScriptParser.Main");
const FuncScriptParser_GetLiteralMatch_1 = require("./FuncScriptParser.GetLiteralMatch");
function GetCommentBlock(context, index) {
    let parseNode = null;
    let i = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, index, "//").NextIndex;
    if (i === index) {
        return { ParseNode: null, NextIndex: index };
    }
    const i2 = context.Expression.indexOf("\n", i);
    if (i2 === -1) {
        i = context.Expression.length;
    }
    else {
        i = i2 + 1;
    }
    parseNode = new FuncScriptParser_Main_1.ParseNode(FuncScriptParser_Main_1.ParseNodeType.Comment, index, i - index);
    return { ParseNode: parseNode, NextIndex: i };
}
