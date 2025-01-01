"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.GetUnit = GetUnit;
const FuncScriptParser_GetCaseExpression_1 = require("./FuncScriptParser.GetCaseExpression");
const FuncScriptParser_GetExpInParenthesis_1 = require("./FuncScriptParser.GetExpInParenthesis");
const FuncScriptParser_GetIdentifier_1 = require("./FuncScriptParser.GetIdentifier");
const FuncScriptParser_GetKeyWordLiteral_1 = require("./FuncScriptParser.GetKeyWordLiteral");
const FuncScriptParser_GetKvcExpression_1 = require("./FuncScriptParser.GetKvcExpression");
const FuncScriptParser_GetLambdaExpression_1 = require("./FuncScriptParser.GetLambdaExpression");
const FuncScriptParser_GetListExpression_1 = require("./FuncScriptParser.GetListExpression");
const FuncScriptParser_GetNumber_1 = require("./FuncScriptParser.GetNumber");
const FuncScriptParser_GetPrefixOperator_1 = require("./FuncScriptParser.GetPrefixOperator");
const FuncScriptParser_GetSimpleString_1 = require("./FuncScriptParser.GetSimpleString");
const FuncScriptParser_GetStringTemplate_1 = require("./FuncScriptParser.GetStringTemplate");
const FuncScriptParser_GetSwitchExpression_1 = require("./FuncScriptParser.GetSwitchExpression");
function GetUnit(context, index) {
    let parseNode = null;
    let i;
    const templateResult = (0, FuncScriptParser_GetStringTemplate_1.GetStringTemplate)(context, index);
    i = templateResult.NextIndex;
    if (i > index) {
        parseNode = templateResult.ParseNode;
        return { ParseNode: parseNode, NextIndex: i };
    }
    const simpleStrResult = (0, FuncScriptParser_GetSimpleString_1.GetSimpleString)(context, index);
    i = simpleStrResult.NextIndex;
    if (i > index) {
        parseNode = simpleStrResult.ParseNode;
        return { ParseNode: parseNode, NextIndex: i };
    }
    const numberResult = (0, FuncScriptParser_GetNumber_1.GetNumber)(context, index);
    i = numberResult.NextIndex;
    if (i > index) {
        parseNode = numberResult.ParseNode;
        return { ParseNode: parseNode, NextIndex: i };
    }
    const listExprResult = (0, FuncScriptParser_GetListExpression_1.GetListExpression)(context, index);
    i = listExprResult.NextIndex;
    if (i > index) {
        parseNode = listExprResult.ParseNode;
        return { ParseNode: parseNode, NextIndex: i };
    }
    const kvcExprResult = (0, FuncScriptParser_GetKvcExpression_1.GetKvcExpression)(context, false, index);
    i = kvcExprResult.NextIndex;
    if (i > index) {
        parseNode = kvcExprResult.ParseNode;
        return { ParseNode: parseNode, NextIndex: i };
    }
    const caseExprResult = (0, FuncScriptParser_GetCaseExpression_1.GetCaseExpression)(context, i);
    i = caseExprResult.NextIndex;
    if (i > index) {
        parseNode = caseExprResult.ParseNode;
        return { ParseNode: parseNode, NextIndex: i };
    }
    const switchExprResult = (0, FuncScriptParser_GetSwitchExpression_1.GetSwitchExpression)(context, i);
    i = switchExprResult.NextIndex;
    if (i > index) {
        parseNode = switchExprResult.ParseNode;
        return { ParseNode: parseNode, NextIndex: i };
    }
    const lambdaExprResult = (0, FuncScriptParser_GetLambdaExpression_1.GetLambdaExpression)(context, index);
    i = lambdaExprResult.NextIndex;
    if (i > index) {
        parseNode = lambdaExprResult.ParseNode;
        return { ParseNode: parseNode, NextIndex: i };
    }
    const keywordLiteralResult = (0, FuncScriptParser_GetKeyWordLiteral_1.GetKeyWordLiteral)(context, index);
    i = keywordLiteralResult.NextIndex;
    if (i > index) {
        parseNode = keywordLiteralResult.ParseNode;
        return { ParseNode: parseNode, NextIndex: i };
    }
    const identResult = (0, FuncScriptParser_GetIdentifier_1.GetIdentifier)(context, index, true);
    i = identResult.NextIndex;
    if (i > index) {
        parseNode = identResult.ParseNode;
        return { ParseNode: parseNode, NextIndex: i };
    }
    const expInParenResult = (0, FuncScriptParser_GetExpInParenthesis_1.GetExpInParenthesis)(context, index);
    i = expInParenResult.NextIndex;
    if (i > index) {
        parseNode = expInParenResult.ParseNode;
        return { ParseNode: parseNode, NextIndex: i };
    }
    const prefixOpResult = (0, FuncScriptParser_GetPrefixOperator_1.GetPrefixOperator)(context, index);
    i = prefixOpResult.NextIndex;
    if (i > index) {
        parseNode = prefixOpResult.ParseNode;
        return { ParseNode: parseNode, NextIndex: i };
    }
    return { ParseNode: parseNode, NextIndex: index };
}
