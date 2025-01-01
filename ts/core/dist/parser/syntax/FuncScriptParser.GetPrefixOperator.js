"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.GetPrefixOperator = GetPrefixOperator;
const FuncScriptParser_Main_1 = require("../FuncScriptParser.Main");
const FuncScriptParser_GetCallAndMemberAccess_1 = require("./FuncScriptParser.GetCallAndMemberAccess");
const FuncScriptParser_GetOperator_1 = require("./FuncScriptParser.GetOperator");
const FuncScriptParser_SkipSpace_1 = require("./FuncScriptParser.SkipSpace");
function GetPrefixOperator(context, index) {
    let i = 0;
    let oper = null;
    let opNode = null;
    const matchResult = (0, FuncScriptParser_GetOperator_1.GetOperator)(context, FuncScriptParser_Main_1.s_prefixOp.map(x => x[0]), index);
    i = matchResult.NextIndex;
    oper = matchResult.MatchedOp;
    if (i === index) {
        return { ParseNode: null, NextIndex: index };
    }
    if (!oper) {
        context.SyntaxErrors.push(new FuncScriptParser_Main_1.SyntaxErrorData(index, i - index, `Prefix operator not defined`));
        return { ParseNode: null, NextIndex: index };
    }
    i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, i).NextIndex;
    const operandRes = (0, FuncScriptParser_GetCallAndMemberAccess_1.GetCallAndMemberAccess)(context, i);
    if (operandRes.NextIndex === i) {
        context.SyntaxErrors.push(new FuncScriptParser_Main_1.SyntaxErrorData(i, 0, `Operand for ${oper} expected`));
        return { ParseNode: null, NextIndex: index };
    }
    i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, operandRes.NextIndex).NextIndex;
    const parseNode = new FuncScriptParser_Main_1.ParseNode(FuncScriptParser_Main_1.ParseNodeType.PrefixOperatorExpression, index, i - index, [matchResult.ParseNode, operandRes.ParseNode]);
    return { ParseNode: parseNode, NextIndex: i };
}
