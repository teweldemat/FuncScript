"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.GetInfixExpressionSingleOp = GetInfixExpressionSingleOp;
const FuncScriptParser_Main_1 = require("../FuncScriptParser.Main");
const FuncScriptParser_GetCallAndMemberAccess_1 = require("./FuncScriptParser.GetCallAndMemberAccess");
const FuncScriptParser_GetLiteralMatch_1 = require("./FuncScriptParser.GetLiteralMatch");
const FuncScriptParser_GetOperator_1 = require("./FuncScriptParser.GetOperator");
const FuncScriptParser_SkipSpace_1 = require("./FuncScriptParser.SkipSpace");
function GetInfixExpressionSingleOp(context, level, candidates, index) {
    let parseNode = null;
    let i = index;
    while (true) {
        let i2;
        let operatorNode = null;
        let symbol = null;
        if (parseNode === null) {
            let result;
            if (level === 0) {
                result = (0, FuncScriptParser_GetCallAndMemberAccess_1.GetCallAndMemberAccess)(context, i);
            }
            else {
                result = GetInfixExpressionSingleOp(context, level - 1, FuncScriptParser_Main_1.s_operatorSymols[level - 1], i);
            }
            parseNode = result.ParseNode;
            i2 = result.NextIndex;
            if (i2 === i)
                return { ParseNode: null, NextIndex: i };
            i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, i2).NextIndex;
            continue;
        }
        const indexBeforeOperator = i;
        const operatorResult = (0, FuncScriptParser_GetOperator_1.GetOperator)(context, candidates, i);
        symbol = operatorResult.MatchedOp;
        operatorNode = operatorResult.ParseNode;
        i2 = operatorResult.NextIndex;
        if (i2 === i)
            break;
        i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, i2).NextIndex;
        const operandNodes = [];
        operandNodes.push(parseNode);
        while (true) {
            let nextOperandNode = null;
            let nextOperandResult;
            if (level === 0) {
                nextOperandResult = (0, FuncScriptParser_GetCallAndMemberAccess_1.GetCallAndMemberAccess)(context, i);
            }
            else {
                nextOperandResult = GetInfixExpressionSingleOp(context, level - 1, FuncScriptParser_Main_1.s_operatorSymols[level - 1], i);
            }
            nextOperandNode = nextOperandResult.ParseNode;
            i2 = nextOperandResult.NextIndex;
            if (i2 === i)
                return { ParseNode: null, NextIndex: indexBeforeOperator };
            operandNodes.push(nextOperandNode);
            i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, i2).NextIndex;
            i2 = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, i, symbol).NextIndex;
            if (i2 === i)
                break;
            i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, i2).NextIndex;
        }
        if (operandNodes.length > 1 && symbol === "|") {
            if (operandNodes.length > 2) {
                context.SyntaxErrors.push(new FuncScriptParser_Main_1.SyntaxErrorData(i, 0, "Only two parameters expected for | "));
                return { ParseNode: null, NextIndex: i };
            }
            parseNode = new FuncScriptParser_Main_1.ParseNode(FuncScriptParser_Main_1.ParseNodeType.InfixExpression, parseNode.Pos, operandNodes[operandNodes.length - 1].Pos + operandNodes[operandNodes.length - 1].Length - parseNode.Length);
        }
        else {
            parseNode = new FuncScriptParser_Main_1.ParseNode(FuncScriptParser_Main_1.ParseNodeType.InfixExpression, parseNode.Pos, operandNodes[operandNodes.length - 1].Pos + operandNodes[operandNodes.length - 1].Length - parseNode.Length);
        }
    }
    return { ParseNode: parseNode, NextIndex: i };
}
