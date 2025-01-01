import { ParseContext, ParseNode, ParseNodeType, ParseResult, SyntaxErrorData } from "../FuncScriptParser.Main";
import { GetExpression } from "./FuncScriptParser.GetExpression";
import { GetLiteralMatch } from "./FuncScriptParser.GetLiteralMatch";
import { SkipSpace } from "./FuncScriptParser.SkipSpace";

export function GetStringTemplate(context: ParseContext, index: number): ParseResult {
    let result = GetStringTemplateWithDelimiter(context, `"`, index);
    if (result.NextIndex > index) {
        return result;
    }
    return GetStringTemplateWithDelimiter(context, `'`, index);
}

export function GetStringTemplateWithDelimiter(context: ParseContext, delimiter: string, index: number): ParseResult {
    let parseNode: ParseNode | null = null;
    const nodeParts: ParseNode[] = [];

    let i = GetLiteralMatch(context, index, `f${delimiter}`).NextIndex;
    if (i === index) {
        return { ParseNode: parseNode, NextIndex: index };
    }

    let lastIndex = i;
    let sb = '';

    while (true) {
        let i2 = GetLiteralMatch(context, i, "\\\\").NextIndex;
        if (i2 > i) {
            i = i2;
            sb += '\\';
            continue;
        }

        i2 = GetLiteralMatch(context, i, "\\n").NextIndex;
        if (i2 > i) {
            i = i2;
            sb += '\n';
            continue;
        }

        i2 = GetLiteralMatch(context, i, "\\t").NextIndex;
        if (i2 > i) {
            i = i2;
            sb += '\t';
            continue;
        }

        i2 = GetLiteralMatch(context, i, `\\${delimiter}`).NextIndex;
        if (i2 > i) {
            i = i2;
            sb += delimiter;
            continue;
        }

        i2 = GetLiteralMatch(context, i, "\\{").NextIndex;
        if (i2 > i) {
            i = i2;
            sb += "{";
            continue;
        }

        i2 = GetLiteralMatch(context, i, "{").NextIndex;
        if (i2 > i) {
            if (sb.length > 0) {
                nodeParts.push(new ParseNode(ParseNodeType.LiteralString, lastIndex, i - lastIndex));
                sb = '';
            }
            i = i2;
            i = SkipSpace(context, i).NextIndex;
            const exprResult = GetExpression(context, i);
            if (exprResult.NextIndex === i) {
                context.SyntaxErrors.push(new SyntaxErrorData(i, 0, "expression expected"));
                return {  ParseNode: parseNode, NextIndex: index };
            }

            nodeParts.push(exprResult.ParseNode!);
            i = exprResult.NextIndex;
            i2 = GetLiteralMatch(context, i, "}").NextIndex;
            if (i2 === i) {
                context.SyntaxErrors.push(new SyntaxErrorData(i, 0, "'}' expected"));
                return { ParseNode: parseNode, NextIndex: index };
            }
            i = i2;
            lastIndex = i;
            continue;
        }

        if (i >= context.Expression.length || GetLiteralMatch(context, i, delimiter).NextIndex > i) {
            break;
        }

        sb += context.Expression[i];
        i++;
    }

    if (i > lastIndex) {
        if (sb.length > 0) {
            nodeParts.push(new ParseNode(ParseNodeType.LiteralString, lastIndex, i - lastIndex));
            sb = '';
        }

        nodeParts.push(new ParseNode(ParseNodeType.LiteralString, lastIndex, i - lastIndex));
    }

    let i2 = GetLiteralMatch(context, i, delimiter).NextIndex;
    if (i2 === i) {
        context.SyntaxErrors.push(new SyntaxErrorData(i, 0, `'${delimiter}' expected`));
        return { ParseNode: parseNode, NextIndex: index };
    }

    i = i2;

    if (nodeParts.length === 0) {
        parseNode = new ParseNode(ParseNodeType.LiteralString, index, i - index);
    } else if (nodeParts.length === 1) {
        parseNode = nodeParts[0];
    } else {
        parseNode = new ParseNode(ParseNodeType.StringTemplate, index, i - index, nodeParts);
    }

    return {ParseNode: parseNode, NextIndex: i };
}