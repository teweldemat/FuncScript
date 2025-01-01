import { ParseContext, ParseNode, ParseNodeType, ParseResult, SyntaxErrorData } from "../FuncScriptParser.Main";
import { GetExpression } from "./FuncScriptParser.GetExpression";
import { GetLiteralMatch } from "./FuncScriptParser.GetLiteralMatch";
import { SkipSpace } from "./FuncScriptParser.SkipSpace";

function GetSpaceSepratedListExpression(context: ParseContext, index: number): ParseResult {
    let parseNode: ParseNode | null = null;
    let i = SkipSpace(context, index).NextIndex;
    const tokenStart = i;
    const nodeListItems: ParseNode[] = [];
    let nodeFirstItem: ParseNode | null = null;
    let i2: number;

    // Use parentheses for destructuring and ensure variables can be reassigned
    ({ ParseNode: nodeFirstItem, NextIndex: i2 } = GetExpression(context, i));

    if (i2 > i) {
        nodeListItems.push(nodeFirstItem!);
        i = i2;
        do {
            i2 = GetLiteralMatch(context, i, " ").NextIndex;
            if (i2 === i) break;
            i = i2;
            i = SkipSpace(context, i).NextIndex;

            // Destructure properly or adjust depending on the function's return
            const { ParseNode: nodeOtherItem, NextIndex: newIndex } = GetExpression(context, i);
            if (newIndex === i) break;
            nodeListItems.push(nodeOtherItem!);
            i = newIndex;
        } while (true);
    }

    parseNode = new ParseNode(ParseNodeType.List, index, i - tokenStart, nodeListItems);
    return { ParseNode: parseNode, NextIndex: i };
}