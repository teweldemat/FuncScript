"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.GetKvcItem = GetKvcItem;
const FuncScriptParser_GetIdentifier_1 = require("./FuncScriptParser.GetIdentifier");
const FuncScriptParser_GetKeyValuePair_1 = require("./FuncScriptParser.GetKeyValuePair");
const FuncScriptParser_GetReturnDefinition_1 = require("./FuncScriptParser.GetReturnDefinition");
const FuncScriptParser_GetSimpleString_1 = require("./FuncScriptParser.GetSimpleString");
function GetKvcItem(context, nakedKvc, index) {
    let parseNode = null;
    const result = (0, FuncScriptParser_GetKeyValuePair_1.GetKeyValuePair)(context, index);
    if (result.NextIndex > index) {
        return { ParseNode: result.ParseNode, NextIndex: result.NextIndex };
    }
    const returnDefResult = (0, FuncScriptParser_GetReturnDefinition_1.GetReturnDefinition)(context, index);
    if (returnDefResult.NextIndex > index) {
        return { ParseNode: returnDefResult.ParseNode, NextIndex: returnDefResult.NextIndex };
    }
    if (!nakedKvc) {
        const identifierResult = (0, FuncScriptParser_GetIdentifier_1.GetIdentifier)(context, index, false);
        if (identifierResult.NextIndex > index) {
            return { ParseNode: identifierResult.ParseNode, NextIndex: identifierResult.NextIndex };
        }
        const simpleStringResult = (0, FuncScriptParser_GetSimpleString_1.GetSimpleString)(context, index);
        if (simpleStringResult.NextIndex > index) {
            return { ParseNode: simpleStringResult.ParseNode, NextIndex: simpleStringResult.NextIndex };
        }
    }
    return { ParseNode: null, NextIndex: index };
}
