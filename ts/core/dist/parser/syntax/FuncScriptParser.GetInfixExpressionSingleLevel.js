"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.GetInfixExpressionSingleLevel = GetInfixExpressionSingleLevel;
const FuncScriptParser_Main_1 = require("../FuncScriptParser.Main");
const FuncScriptParser_GetInfixFunctionCall_1 = require("./FuncScriptParser.GetInfixFunctionCall");
const FuncScriptParser_GetOperator_1 = require("./FuncScriptParser.GetOperator");
const FuncScriptParser_SkipSpace_1 = require("./FuncScriptParser.SkipSpace");
function GetInfixExpressionSingleLevel(context, level, candidates, index) {
    let parseNode = null;
    let i = index;
    while (true) {
        let i2;
        let symbol = null;
        let operatorNode = null;
        if (parseNode === null) {
            let result;
            if (level === 0) {
                result = (0, FuncScriptParser_GetInfixFunctionCall_1.GetInfixFunctionCall)(context, i);
            }
            else {
                result = GetInfixExpressionSingleLevel(context, level - 1, FuncScriptParser_Main_1.s_operatorSymols[level - 1], i);
            }
            parseNode = result.ParseNode;
            i2 = result.NextIndex;
            if (i2 === i) {
                return { ParseNode: parseNode, NextIndex: i };
            }
            i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, i2).NextIndex;
            continue;
        }
        const indexBeforeOperator = i;
        const operatorResult = (0, FuncScriptParser_GetOperator_1.GetOperator)(context, candidates, i);
        symbol = operatorResult.MatchedOp;
        operatorNode = operatorResult.ParseNode;
        i2 = operatorResult.NextIndex;
        if (i2 === i) {
            break;
        }
        i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, i2).NextIndex;
        const infixComponentNodes = [parseNode];
        infixComponentNodes.push(operatorNode);
        while (true) {
            let nextOperandNode;
            if (level === 0) {
                const result = (0, FuncScriptParser_GetInfixFunctionCall_1.GetInfixFunctionCall)(context, i);
                nextOperandNode = result.ParseNode;
                i2 = result.NextIndex;
            }
            else {
                const result = GetInfixExpressionSingleLevel(context, level - 1, FuncScriptParser_Main_1.s_operatorSymols[level - 1], i);
                nextOperandNode = result.ParseNode;
                i2 = result.NextIndex;
            }
            if (i2 === i) {
                return { ParseNode: parseNode, NextIndex: indexBeforeOperator };
            }
            infixComponentNodes.push(nextOperandNode);
            i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, i2).NextIndex;
            const nextOperatorResult = (0, FuncScriptParser_GetOperator_1.GetOperator)(context, [symbol], i);
            i2 = nextOperatorResult.NextIndex;
            if (i2 === i) {
                break;
            }
            i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, i2).NextIndex;
        }
        if (infixComponentNodes.length > 1) {
            parseNode = new FuncScriptParser_Main_1.ParseNode(FuncScriptParser_Main_1.ParseNodeType.InfixExpression, parseNode.Pos, infixComponentNodes[infixComponentNodes.length - 1].Pos + infixComponentNodes[infixComponentNodes.length - 1].Length - parseNode.Pos, infixComponentNodes);
        }
    }
    return { ParseNode: parseNode, NextIndex: i };
}
