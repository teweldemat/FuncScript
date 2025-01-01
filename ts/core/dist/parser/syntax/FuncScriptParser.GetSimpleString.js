"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.GetSimpleString = GetSimpleString;
const FuncScriptParser_Main_1 = require("../FuncScriptParser.Main");
const FuncScriptParser_GetLiteralMatch_1 = require("./FuncScriptParser.GetLiteralMatch");
function GetSimpleString(context, index) {
    let res = GetSimpleStringInternal(context, "\"", index);
    if (res.NextIndex > index)
        return res;
    return GetSimpleStringInternal(context, "'", index);
}
function GetSimpleStringInternal(context, delimator, index) {
    let parseNode = null;
    let str = null;
    let i = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, index, delimator).NextIndex;
    if (i === index)
        return { Str: null, ParseNode: null, NextIndex: index };
    let i2;
    let sb = [];
    while (true) {
        i2 = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, i, "\\n").NextIndex;
        if (i2 > i) {
            i = i2;
            sb.push('\n');
            continue;
        }
        i2 = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, i, "\\t").NextIndex;
        if (i2 > i) {
            i = i2;
            sb.push('\t');
            continue;
        }
        i2 = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, i, "\\\\").NextIndex;
        if (i2 > i) {
            i = i2;
            sb.push('\\');
            continue;
        }
        i2 = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, i, "\\u").NextIndex;
        if (i2 > i) {
            if (i + 6 <= context.Expression.length) {
                const unicodeStr = context.Expression.substring(i + 2, i + 6);
                const charValue = parseInt(unicodeStr, 16);
                if (!isNaN(charValue)) {
                    sb.push(String.fromCharCode(charValue));
                    i += 6;
                    continue;
                }
            }
        }
        i2 = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, i, "\\" + delimator).NextIndex;
        if (i2 > i) {
            sb.push(delimator);
            i = i2;
            continue;
        }
        if (i >= context.Expression.length || (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, i, delimator).NextIndex > i)
            break;
        sb.push(context.Expression[i]);
        i++;
    }
    i2 = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, i, delimator).NextIndex;
    if (i2 === i) {
        context.SyntaxErrors.push(new FuncScriptParser_Main_1.SyntaxErrorData(i, 0, `'${delimator}' expected`));
        return { Str: null, ParseNode: null, NextIndex: index };
    }
    i = i2;
    str = sb.join('');
    parseNode = new FuncScriptParser_Main_1.ParseNode(FuncScriptParser_Main_1.ParseNodeType.LiteralString, index, i - index);
    return { Str: str, ParseNode: parseNode, NextIndex: i };
}
