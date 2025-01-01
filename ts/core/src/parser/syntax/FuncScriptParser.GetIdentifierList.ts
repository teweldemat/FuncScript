import { ParseContext, ParseNode, ParseNodeType, ParseResult } from "../FuncScriptParser.Main";
import { GetIdentifier } from "./FuncScriptParser.GetIdentifier";
import { SkipSpace } from "./FuncScriptParser.SkipSpace";

export function GetIdentifierList(context: ParseContext, index: number): ParseResult {
    let i = SkipSpace(context, index).NextIndex;

    if (i >= context.Expression.length || context.Expression[i++] !== '(') {
        return { ParseNode: null, NextIndex: index };
    }

    const parseNodes: ParseNode[] = [];

    i = SkipSpace(context, i).NextIndex;
    const { ParseNode:nodeIden,NextIndex:i2} = GetIdentifier(context, i, false);

    if (i2 > i) {
        parseNodes.push(nodeIden!);
        i = i2;

        i = SkipSpace(context, i).NextIndex;
        while (i < context.Expression.length) {
            if (context.Expression[i] !== ',') break;

            i++;
            i = SkipSpace(context, i).NextIndex;
            const {ParseNode:nodeIdenNext,NextIndex: i3} = GetIdentifier(context, i, false);

            if (i3 === i) {
                return { ParseNode: null, NextIndex: index };
            }

            parseNodes.push(nodeIdenNext!);
            i = i3;
            i = SkipSpace(context, i).NextIndex;
        }
    }

    if (i >= context.Expression.length || context.Expression[i++] !== ')') {
        return { ParseNode: null, NextIndex: index };
    }

    const parseNode = new ParseNode(ParseNodeType.IdentifierList, index, i - index, parseNodes);
    return { ParseNode: parseNode, NextIndex: i };
}