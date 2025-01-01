"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.GetCallAndMemberAccess = GetCallAndMemberAccess;
const FuncScriptParser_Main_1 = require("../FuncScriptParser.Main");
const FuncScriptParser_GetFunctionCallParametersList_1 = require("./FuncScriptParser.GetFunctionCallParametersList");
const FuncScriptParser_GetKvcExpression_1 = require("./FuncScriptParser.GetKvcExpression");
const FuncScriptParser_GetMemberAccess_1 = require("./FuncScriptParser.GetMemberAccess");
const FuncScriptParser_GetUnit_1 = require("./FuncScriptParser.GetUnit");
const FuncScriptParser_SkipSpace_1 = require("./FuncScriptParser.SkipSpace");
function GetCallAndMemberAccess(context, index) {
    let parseNode = null;
    const i1 = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, index).NextIndex;
    let { ParseNode: tempParseNode, NextIndex: i } = (0, FuncScriptParser_GetUnit_1.GetUnit)(context, i1);
    if (i === index) {
        return { ParseNode: null, NextIndex: index };
    }
    parseNode = tempParseNode;
    while (true) {
        let { ParseNode: nodeParList, NextIndex: i2 } = (0, FuncScriptParser_GetFunctionCallParametersList_1.GetFunctionCallParametersList)(context, i);
        if (i2 > i) {
            i = i2;
            parseNode = new FuncScriptParser_Main_1.ParseNode(FuncScriptParser_Main_1.ParseNodeType.FunctionCall, index, i - index, [parseNode, nodeParList]);
            continue;
        }
        let { ParseNode: nodeMemberAccess, NextIndex: i2Member } = (0, FuncScriptParser_GetMemberAccess_1.GetMemberAccess)(context, i);
        if (i2Member > i) {
            i = i2Member;
            parseNode = new FuncScriptParser_Main_1.ParseNode(FuncScriptParser_Main_1.ParseNodeType.MemberAccess, index, i - index, [parseNode, nodeMemberAccess]);
            continue;
        }
        let { ParseNode: nodeKvc, NextIndex: i2Kvc } = (0, FuncScriptParser_GetKvcExpression_1.GetKvcExpression)(context, false, i);
        if (i2Kvc > i) {
            i = i2Kvc;
            parseNode = new FuncScriptParser_Main_1.ParseNode(FuncScriptParser_Main_1.ParseNodeType.Selection, index, i - index, [parseNode, nodeKvc]);
            continue;
        }
        return { ParseNode: parseNode, NextIndex: i };
    }
}
