import { ParseContext, ParseNode, ParseNodeType, ParseResult, SyntaxErrorData } from "../FuncScriptParser.Main";
import { GetLiteralMatch } from "./FuncScriptParser.GetLiteralMatch";

export interface GetSimpleStringResult extends ParseResult {
    Str: string | null;
    ParseNode: ParseNode | null;
    NextIndex: number;
}

export function GetSimpleString(context: ParseContext, index: number): GetSimpleStringResult {
    let res = GetSimpleStringInternal(context, "\"", index);
    if (res.NextIndex > index) return res;
    return GetSimpleStringInternal(context, "'", index);
}

function GetSimpleStringInternal(context: ParseContext, delimator: string, index: number): GetSimpleStringResult {
    let parseNode: ParseNode | null = null;
    let str: string | null = null;
    let i = GetLiteralMatch(context, index, delimator).NextIndex;
    if (i === index) return { Str: null, ParseNode: null, NextIndex: index };

    let i2: number;
    let sb: string[] = [];

    while (true) {
        i2 = GetLiteralMatch(context, i, "\\n").NextIndex;
        if (i2 > i) {
            i = i2;
            sb.push('\n');
            continue;
        }

        i2 = GetLiteralMatch(context, i, "\\t").NextIndex;
        if (i2 > i) {
            i = i2;
            sb.push('\t');
            continue;
        }

        i2 = GetLiteralMatch(context, i, "\\\\").NextIndex;
        if (i2 > i) {
            i = i2;
            sb.push('\\');
            continue;
        }

        i2 = GetLiteralMatch(context, i, "\\u").NextIndex;
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

        i2 = GetLiteralMatch(context, i, "\\" + delimator).NextIndex;
        if (i2 > i) {
            sb.push(delimator);
            i = i2;
            continue;
        }

        if (i >= context.Expression.length || GetLiteralMatch(context, i, delimator).NextIndex > i) break;
        sb.push(context.Expression[i]);
        i++;
    }

    i2 = GetLiteralMatch(context, i, delimator).NextIndex;
    if (i2 === i) {
        context.SyntaxErrors.push(new SyntaxErrorData(i, 0, `'${delimator}' expected`));
        return { Str: null, ParseNode: null, NextIndex: index };
    }

    i = i2;
    str = sb.join('');
    parseNode = new ParseNode(ParseNodeType.LiteralString, index, i - index);
    return { Str: str, ParseNode: parseNode, NextIndex: i };
}