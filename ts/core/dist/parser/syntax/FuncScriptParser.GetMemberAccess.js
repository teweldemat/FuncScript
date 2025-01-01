"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.GetMemberAccess = GetMemberAccess;
exports.GetMemberAccessInternal = GetMemberAccessInternal;
const FuncScriptParser_Main_1 = require("../FuncScriptParser.Main");
const FuncScriptParser_GetIdentifier_1 = require("./FuncScriptParser.GetIdentifier");
const FuncScriptParser_GetLiteralMatch_1 = require("./FuncScriptParser.GetLiteralMatch");
const FuncScriptParser_SkipSpace_1 = require("./FuncScriptParser.SkipSpace");
function GetMemberAccess(context, index) {
    const result = GetMemberAccessInternal(context, ".", index);
    if (result.NextIndex === index) {
        return GetMemberAccessInternal(context, "?.", index);
    }
    return result;
}
function GetMemberAccessInternal(context, oper, index) {
    let parseNode = null;
    let i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, index).NextIndex;
    const i2 = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, i, oper).NextIndex;
    if (i2 === i) {
        return { ParseNode: null, NextIndex: index };
    }
    i = i2;
    i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, i).NextIndex;
    const identifierResult = (0, FuncScriptParser_GetIdentifier_1.GetIdentifier)(context, i, false);
    if (identifierResult.NextIndex === i) {
        context.SyntaxErrors.push(new FuncScriptParser_Main_1.SyntaxErrorData(i, 0, "member identifier expected"));
        return { ParseNode: null, NextIndex: index };
    }
    i = identifierResult.NextIndex;
    return { ParseNode: identifierResult.ParseNode, NextIndex: i };
}
