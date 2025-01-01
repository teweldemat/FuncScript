"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.GetKvcExpression = GetKvcExpression;
const FuncScriptParser_Main_1 = require("../FuncScriptParser.Main");
const FuncScriptParser_GetKvcItem_1 = require("./FuncScriptParser.GetKvcItem");
const FuncScriptParser_GetLiteralMatch_1 = require("./FuncScriptParser.GetLiteralMatch");
const FuncScriptParser_SkipSpace_1 = require("./FuncScriptParser.SkipSpace");
function GetKvcExpression(context, nakdeMode, index) {
    const syntaxErrors = context.SyntaxErrors;
    let parseNode = null;
    let i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, index).NextIndex;
    let i2;
    if (!nakdeMode) {
        i2 = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, i, "{").NextIndex;
        if (i2 === i) {
            return { ParseNode: null, NextIndex: index };
        }
        i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, i2).NextIndex;
    }
    const nodeItems = [];
    do {
        if (nodeItems.length > 0) {
            i2 = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatchMultiple)(context, i, [",", ";"]).NextIndex;
            if (i2 === i) {
                break;
            }
            i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, i2).NextIndex;
        }
        const kvcItemResult = (0, FuncScriptParser_GetKvcItem_1.GetKvcItem)(context, nakdeMode, i);
        i2 = kvcItemResult.NextIndex;
        const nodeOtherItem = kvcItemResult.ParseNode;
        if (i2 === i) {
            break;
        }
        nodeItems.push(nodeOtherItem);
        i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, i2).NextIndex;
    } while (true);
    if (!nakdeMode) {
        i2 = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, i, "}").NextIndex;
        if (i2 === i) {
            syntaxErrors.push(new FuncScriptParser_Main_1.SyntaxErrorData(i, 0, "'}' expected"));
            return { ParseNode: null, NextIndex: index };
        }
        i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, i2).NextIndex;
    }
    parseNode = new FuncScriptParser_Main_1.ParseNode(FuncScriptParser_Main_1.ParseNodeType.KeyValueCollection, index, i - index, nodeItems);
    return { ParseNode: parseNode, NextIndex: i };
}
