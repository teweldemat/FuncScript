"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.GetInfixFunctionCall = GetInfixFunctionCall;
const FuncScriptParser_Main_1 = require("../FuncScriptParser.Main");
const FuncScriptParser_GetCallAndMemberAccess_1 = require("./FuncScriptParser.GetCallAndMemberAccess");
const FuncScriptParser_GetIdentifier_1 = require("./FuncScriptParser.GetIdentifier");
const FuncScriptParser_GetLiteralMatch_1 = require("./FuncScriptParser.GetLiteralMatch");
const FuncScriptParser_SkipSpace_1 = require("./FuncScriptParser.SkipSpace");
function GetInfixFunctionCall(context, index) {
    const childNodes = [];
    const result = (0, FuncScriptParser_GetCallAndMemberAccess_1.GetCallAndMemberAccess)(context, index);
    if (result.NextIndex === index) {
        return { ParseNode: null, NextIndex: index };
    }
    var parseNode = result.ParseNode;
    childNodes.push(parseNode);
    let i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, result.NextIndex).NextIndex;
    const identifierResult = (0, FuncScriptParser_GetIdentifier_1.GetIdentifier)(context, i, false);
    if (identifierResult.NextIndex === i) {
        return { ParseNode: parseNode, NextIndex: i };
    }
    childNodes.push(identifierResult.ParseNode);
    i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, identifierResult.NextIndex).NextIndex;
    const secondParamResult = (0, FuncScriptParser_GetCallAndMemberAccess_1.GetCallAndMemberAccess)(context, i);
    if (secondParamResult.NextIndex === i) {
        context.SyntaxErrors.push(new FuncScriptParser_Main_1.SyntaxErrorData(i, 0, `Right side operand expected for ${identifierResult.Iden}`));
        return { ParseNode: null, NextIndex: index };
    }
    childNodes.push(secondParamResult.ParseNode);
    i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, secondParamResult.NextIndex).NextIndex;
    while (true) {
        const literalMatchResult = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, i, "~");
        if (literalMatchResult.NextIndex === i)
            break;
        i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, literalMatchResult.NextIndex).NextIndex;
        const moreOperandResult = (0, FuncScriptParser_GetCallAndMemberAccess_1.GetCallAndMemberAccess)(context, i);
        if (moreOperandResult.NextIndex === i)
            break;
        i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, moreOperandResult.NextIndex).NextIndex;
        childNodes.push(moreOperandResult.ParseNode);
    }
    if (childNodes.length < 2) {
        return { ParseNode: null, NextIndex: index };
    }
    parseNode = new FuncScriptParser_Main_1.ParseNode(FuncScriptParser_Main_1.ParseNodeType.GeneralInfixExpression, childNodes[0].Pos, childNodes[childNodes.length - 1].Pos + childNodes[childNodes.length - 1].Length - childNodes[0].Pos, childNodes);
    return { ParseNode: parseNode, NextIndex: i };
}
