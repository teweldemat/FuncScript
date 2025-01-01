"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.GetIdentifier = GetIdentifier;
const FuncScriptParser_Main_1 = require("../FuncScriptParser.Main");
const FuncScriptParser_GetLiteralMatch_1 = require("./FuncScriptParser.GetLiteralMatch");
const FuncScriptParser_IsIdentfierFirstChar_1 = require("./FuncScriptParser.IsIdentfierFirstChar");
const FuncScriptParser_IsIdentfierOtherChar_1 = require("./FuncScriptParser.IsIdentfierOtherChar");
function GetIdentifier(context, index, supportParentRef) {
    let iden = null;
    let idenLower = null;
    let parentRef = false;
    let parseNode = null;
    const exp = context.Expression;
    if (index >= exp.length) {
        return { Iden: iden, IdenLower: idenLower, ParentRef: parentRef, ParseNode: null, NextIndex: index };
    }
    let i1 = index;
    let i = i1;
    if (supportParentRef) {
        const i2 = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, i, "^").NextIndex;
        if (i2 > i) {
            parentRef = true;
            i1 = i2;
            i = i2;
        }
    }
    if (!(0, FuncScriptParser_IsIdentfierFirstChar_1.IsIdentifierFirstChar)(exp[i])) {
        return { Iden: iden, IdenLower: idenLower, ParentRef: parentRef, ParseNode: null, NextIndex: index };
    }
    i++;
    while (i < exp.length && (0, FuncScriptParser_IsIdentfierOtherChar_1.IsIdentfierOtherChar)(exp[i])) {
        i++;
    }
    iden = exp.substring(i1, i);
    idenLower = iden.toLowerCase();
    if (FuncScriptParser_Main_1.s_KeyWords.has(idenLower)) {
        return { Iden: iden, IdenLower: idenLower, ParentRef: parentRef, ParseNode: null, NextIndex: index };
    }
    parseNode = new FuncScriptParser_Main_1.ParseNode(FuncScriptParser_Main_1.ParseNodeType.Identifier, index, i - index);
    return { Iden: iden, IdenLower: idenLower, ParentRef: parentRef, ParseNode: parseNode, NextIndex: i };
}
