"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.GetStringTemplate = GetStringTemplate;
exports.GetStringTemplateWithDelimiter = GetStringTemplateWithDelimiter;
const FuncScriptParser_Main_1 = require("../FuncScriptParser.Main");
const FuncScriptParser_GetExpression_1 = require("./FuncScriptParser.GetExpression");
const FuncScriptParser_GetLiteralMatch_1 = require("./FuncScriptParser.GetLiteralMatch");
const FuncScriptParser_SkipSpace_1 = require("./FuncScriptParser.SkipSpace");
function GetStringTemplate(context, index) {
    let result = GetStringTemplateWithDelimiter(context, `"`, index);
    if (result.NextIndex > index) {
        return result;
    }
    return GetStringTemplateWithDelimiter(context, `'`, index);
}
function GetStringTemplateWithDelimiter(context, delimiter, index) {
    let parseNode = null;
    const nodeParts = [];
    let i = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, index, `f${delimiter}`).NextIndex;
    if (i === index) {
        return { ParseNode: parseNode, NextIndex: index };
    }
    let lastIndex = i;
    let sb = '';
    while (true) {
        let i2 = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, i, "\\\\").NextIndex;
        if (i2 > i) {
            i = i2;
            sb += '\\';
            continue;
        }
        i2 = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, i, "\\n").NextIndex;
        if (i2 > i) {
            i = i2;
            sb += '\n';
            continue;
        }
        i2 = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, i, "\\t").NextIndex;
        if (i2 > i) {
            i = i2;
            sb += '\t';
            continue;
        }
        i2 = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, i, `\\${delimiter}`).NextIndex;
        if (i2 > i) {
            i = i2;
            sb += delimiter;
            continue;
        }
        i2 = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, i, "\\{").NextIndex;
        if (i2 > i) {
            i = i2;
            sb += "{";
            continue;
        }
        i2 = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, i, "{").NextIndex;
        if (i2 > i) {
            if (sb.length > 0) {
                nodeParts.push(new FuncScriptParser_Main_1.ParseNode(FuncScriptParser_Main_1.ParseNodeType.LiteralString, lastIndex, i - lastIndex));
                sb = '';
            }
            i = i2;
            i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, i).NextIndex;
            const exprResult = (0, FuncScriptParser_GetExpression_1.GetExpression)(context, i);
            if (exprResult.NextIndex === i) {
                context.SyntaxErrors.push(new FuncScriptParser_Main_1.SyntaxErrorData(i, 0, "expression expected"));
                return { ParseNode: parseNode, NextIndex: index };
            }
            nodeParts.push(exprResult.ParseNode);
            i = exprResult.NextIndex;
            i2 = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, i, "}").NextIndex;
            if (i2 === i) {
                context.SyntaxErrors.push(new FuncScriptParser_Main_1.SyntaxErrorData(i, 0, "'}' expected"));
                return { ParseNode: parseNode, NextIndex: index };
            }
            i = i2;
            lastIndex = i;
            continue;
        }
        if (i >= context.Expression.length || (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, i, delimiter).NextIndex > i) {
            break;
        }
        sb += context.Expression[i];
        i++;
    }
    if (i > lastIndex) {
        if (sb.length > 0) {
            nodeParts.push(new FuncScriptParser_Main_1.ParseNode(FuncScriptParser_Main_1.ParseNodeType.LiteralString, lastIndex, i - lastIndex));
            sb = '';
        }
        nodeParts.push(new FuncScriptParser_Main_1.ParseNode(FuncScriptParser_Main_1.ParseNodeType.LiteralString, lastIndex, i - lastIndex));
    }
    let i2 = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, i, delimiter).NextIndex;
    if (i2 === i) {
        context.SyntaxErrors.push(new FuncScriptParser_Main_1.SyntaxErrorData(i, 0, `'${delimiter}' expected`));
        return { ParseNode: parseNode, NextIndex: index };
    }
    i = i2;
    if (nodeParts.length === 0) {
        parseNode = new FuncScriptParser_Main_1.ParseNode(FuncScriptParser_Main_1.ParseNodeType.LiteralString, index, i - index);
    }
    else if (nodeParts.length === 1) {
        parseNode = nodeParts[0];
    }
    else {
        parseNode = new FuncScriptParser_Main_1.ParseNode(FuncScriptParser_Main_1.ParseNodeType.StringTemplate, index, i - index, nodeParts);
    }
    return { ParseNode: parseNode, NextIndex: i };
}
