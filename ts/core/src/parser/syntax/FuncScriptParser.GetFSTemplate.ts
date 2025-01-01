import { ParseContext, ParseNode, ParseNodeType, SyntaxErrorData } from "../FuncScriptParser.Main";
import { GetExpression } from "./FuncScriptParser.GetExpression";
import { GetLiteralMatch } from "./FuncScriptParser.GetLiteralMatch";
import { SkipSpace } from "./FuncScriptParser.SkipSpace";

export function GetFSTemplate(context: ParseContext, index: number) {
    let parseNode: ParseNode | null = null;
    let i = index;
    let i2: number;
    let lastIndex = i;
    let nodeParts:ParseNode[] =[]

    while (true) {
        i2 = GetLiteralMatch(context, i, "$${").NextIndex;
        if (i2 > i) {
            i = i2;
        }

        i2 = GetLiteralMatch(context, i, "${").NextIndex;
        if (i2 > i) {
            i = i2;

            i = SkipSpace(context, i).NextIndex;
            const resultExpr = GetExpression(context, i);
            const nodeExpr = resultExpr.ParseNode;
            i2 = resultExpr.NextIndex;

            if (i2 === i) {
                context.SyntaxErrors.push(new SyntaxErrorData(i, 0, "expression expected"));
                return { ParseNode: null, NextIndex: index };
            }

            i = SkipSpace(context, i).NextIndex;
            nodeParts.push(nodeExpr!);
            i = i2;

            i2 = GetLiteralMatch(context, i, "}").NextIndex;
            if (i2 === i) {
                context.SyntaxErrors.push(new SyntaxErrorData(i, 0, "'}' expected"));
                return { ParseNode: null, NextIndex: index };
            }

            i = i2;
            lastIndex = i;
            if (i < context.Expression.length)
                continue;
            else
                break;
        }

        i++;
        if (i === context.Expression.length)
            break;
    }

    if (nodeParts.length === 0) {
        parseNode = new ParseNode(ParseNodeType.LiteralString, index, i - index);
    } else if (nodeParts.length === 1) {
        parseNode = nodeParts[0];
    } else {
        parseNode = new ParseNode(ParseNodeType.StringTemplate, index, i - index, nodeParts);
    }

    return { ParseNode: parseNode, NextIndex: i };
}