"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.GetIdentifierList = GetIdentifierList;
const FuncScriptParser_Main_1 = require("../FuncScriptParser.Main");
const FuncScriptParser_GetIdentifier_1 = require("./FuncScriptParser.GetIdentifier");
const FuncScriptParser_SkipSpace_1 = require("./FuncScriptParser.SkipSpace");
function GetIdentifierList(context, index) {
    let i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, index).NextIndex;
    if (i >= context.Expression.length || context.Expression[i++] !== '(') {
        return { ParseNode: null, NextIndex: index };
    }
    const parseNodes = [];
    i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, i).NextIndex;
    const { ParseNode: nodeIden, NextIndex: i2 } = (0, FuncScriptParser_GetIdentifier_1.GetIdentifier)(context, i, false);
    if (i2 > i) {
        parseNodes.push(nodeIden);
        i = i2;
        i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, i).NextIndex;
        while (i < context.Expression.length) {
            if (context.Expression[i] !== ',')
                break;
            i++;
            i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, i).NextIndex;
            const { ParseNode: nodeIdenNext, NextIndex: i3 } = (0, FuncScriptParser_GetIdentifier_1.GetIdentifier)(context, i, false);
            if (i3 === i) {
                return { ParseNode: null, NextIndex: index };
            }
            parseNodes.push(nodeIdenNext);
            i = i3;
            i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, i).NextIndex;
        }
    }
    if (i >= context.Expression.length || context.Expression[i++] !== ')') {
        return { ParseNode: null, NextIndex: index };
    }
    const parseNode = new FuncScriptParser_Main_1.ParseNode(FuncScriptParser_Main_1.ParseNodeType.IdentifierList, index, i - index, parseNodes);
    return { ParseNode: parseNode, NextIndex: i };
}
