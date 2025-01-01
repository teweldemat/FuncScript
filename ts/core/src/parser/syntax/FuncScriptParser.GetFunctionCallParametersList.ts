import { ParseContext, ParseNode, ParseNodeType, ParseResult, SyntaxErrorData } from "../FuncScriptParser.Main";
import { GetExpression } from "./FuncScriptParser.GetExpression";
import { GetLiteralMatch } from "./FuncScriptParser.GetLiteralMatch";
import { SkipSpace } from "./FuncScriptParser.SkipSpace";

export function GetFunctionCallParametersList(context: ParseContext, index: number): ParseResult {
    const result = getFunctionCallParametersList(context, "(", ")",  index);
    if (result.NextIndex === index) {
        return getFunctionCallParametersList(context, "[", "]", index);
    }
    return result;
}

function getFunctionCallParametersList(context: ParseContext, openBrace: string, closeBrace: string,  index: number): ParseResult {
    let parseNode: ParseNode | null = null;
    let prog: any | null = null;

    let i = SkipSpace(context, index).NextIndex;
    let i2 = GetLiteralMatch(context, i, openBrace).NextIndex;
    if (i === i2) {
        return { ParseNode: null, NextIndex: index };
    }

    i = i2;
    const parseNodes: ParseNode[] = [];

    i = SkipSpace(context, i).NextIndex;
    let exprResult = GetExpression(context, i);

    if (exprResult.NextIndex > i) {
        i = exprResult.NextIndex;
        parseNodes.push(exprResult.ParseNode!);

        do {
            i2 = SkipSpace(context, i).NextIndex;
            if (i2 >= context.Expression.length || context.Expression[i2++] !== ',') {
                break;
            }
            i = i2;
            i = SkipSpace(context, i).NextIndex;
            exprResult = GetExpression(context, i);
            if (exprResult.NextIndex === i) {
                context.SyntaxErrors.push(new SyntaxErrorData(i, 0, "Parameter for call expected"));
                return { ParseNode: null, NextIndex: index };
            }

            i = exprResult.NextIndex;
            parseNodes.push(exprResult.ParseNode!);
        } while (true);
    }

    i = SkipSpace(context, i).NextIndex;
    i2 = GetLiteralMatch(context, i, closeBrace).NextIndex;
    if (i2 === i) {
        context.SyntaxErrors.push(new SyntaxErrorData(i, 0, `'${closeBrace}' expected`));
        return { ParseNode: null, NextIndex: index };
    }

    i = i2;

    parseNode = new ParseNode(ParseNodeType.FunctionParameterList, index, i - index, parseNodes);
    return { ParseNode: parseNode, NextIndex: i };
}