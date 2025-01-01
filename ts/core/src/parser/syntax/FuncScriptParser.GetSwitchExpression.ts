import { KW_SWITCH, ParseContext, ParseNode, ParseNodeType, SyntaxErrorData } from "../FuncScriptParser.Main";
import { GetExpression } from "./FuncScriptParser.GetExpression";
import { GetLiteralMatch, GetLiteralMatchMultiple } from "./FuncScriptParser.GetLiteralMatch";
import { SkipSpace } from "./FuncScriptParser.SkipSpace";

export function GetSwitchExpression(context: ParseContext, index: number) {
    let parseNode: ParseNode | null = null;
    let i = index;
    let i2 = GetLiteralMatch(context, i, KW_SWITCH).NextIndex;

    if (i2 === i) {
        return { ParseNode: null, NextIndex: index };
    }

    i = SkipSpace(context, i2).NextIndex;
    const childNodes: ParseNode[] = [];

    const expressionResult = GetExpression(context, i);

    if (expressionResult.NextIndex === i) {
        context.SyntaxErrors.push(new SyntaxErrorData(i, 1, "Switch selector expected"));
        return { ParseNode: null, NextIndex: index };
    }

    childNodes.push(expressionResult.ParseNode!);
    i = SkipSpace(context, expressionResult.NextIndex).NextIndex;

    while (true) {
        i2 = GetLiteralMatchMultiple(context, i, [",", ";"]).NextIndex;
        if (i2 === i) {
            break;
        }
        i = SkipSpace(context, i2).NextIndex;

        const part1Result = GetExpression(context, i);
        if (part1Result.NextIndex === i) {
            break;
        }

        childNodes.push(part1Result.ParseNode!);
        i = SkipSpace(context, part1Result.NextIndex).NextIndex;

        i2 = GetLiteralMatch(context, i, ":").NextIndex;
        if (i2 === i) {
            break;
        }

        i = SkipSpace(context, i2).NextIndex;

        const part2Result = GetExpression(context, i);
        if (part2Result.NextIndex === i) {
            context.SyntaxErrors.push(new SyntaxErrorData(i, 1, "Selector result expected"));
            return { ParseNode: null, NextIndex: index };
        }

        childNodes.push(part2Result.ParseNode!);
        i = SkipSpace(context, part2Result.NextIndex).NextIndex;
    }

    parseNode = new ParseNode(ParseNodeType.Case, index, i - index);
    parseNode.Children = childNodes;
    return { ParseNode: parseNode, NextIndex: i };
}