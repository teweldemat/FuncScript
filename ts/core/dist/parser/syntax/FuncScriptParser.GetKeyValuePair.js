"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.GetKeyValuePair = GetKeyValuePair;
const FuncScriptParser_Main_1 = require("../FuncScriptParser.Main");
const FuncScriptParser_GetExpression_1 = require("./FuncScriptParser.GetExpression");
const FuncScriptParser_GetIdentifier_1 = require("./FuncScriptParser.GetIdentifier");
const FuncScriptParser_GetLiteralMatch_1 = require("./FuncScriptParser.GetLiteralMatch");
const FuncScriptParser_GetSimpleString_1 = require("./FuncScriptParser.GetSimpleString");
const FuncScriptParser_SkipSpace_1 = require("./FuncScriptParser.SkipSpace");
function GetKeyValuePair(context, index) {
    let parseNode = null;
    let i = index;
    let name;
    let nameLower;
    let nodeName;
    ({ Str: name, ParseNode: nodeName, NextIndex: i } = (0, FuncScriptParser_GetSimpleString_1.GetSimpleString)(context, index));
    if (i === index) {
        ({ Iden: name, IdenLower: nameLower, ParseNode: nodeName, NextIndex: i } = (0, FuncScriptParser_GetIdentifier_1.GetIdentifier)(context, index, false));
        if (i === index) {
            return { ParseNode: null, NextIndex: index };
        }
    }
    else {
        nameLower = name.toLowerCase();
    }
    i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, i).NextIndex;
    let i2 = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, i, ":").NextIndex;
    if (i2 === i) {
        nodeName.NodeType = FuncScriptParser_Main_1.ParseNodeType.Key;
        parseNode = new FuncScriptParser_Main_1.ParseNode(FuncScriptParser_Main_1.ParseNodeType.KeyValuePair, index, i - index, [nodeName]);
        return { ParseNode: parseNode, NextIndex: i };
    }
    i = i2;
    i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, i).NextIndex;
    var nodeExpBlock;
    ({ ParseNode: nodeExpBlock, NextIndex: i2 } = (0, FuncScriptParser_GetExpression_1.GetExpression)(context, i));
    if (i2 === i) {
        context.SyntaxErrors.push(new FuncScriptParser_Main_1.SyntaxErrorData(i, 0, "value expression expected"));
        return { ParseNode: null, NextIndex: index };
    }
    i = i2;
    i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, i).NextIndex;
    nodeName.NodeType = FuncScriptParser_Main_1.ParseNodeType.Key;
    parseNode = new FuncScriptParser_Main_1.ParseNode(FuncScriptParser_Main_1.ParseNodeType.KeyValuePair, index, i - index, [nodeName, nodeExpBlock]);
    return { ParseNode: parseNode, NextIndex: i };
}
