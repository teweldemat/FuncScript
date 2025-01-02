import { ParseContext, ParseResult, SyntaxErrorData, ParseNode, ParseNodeType } from "../FuncScriptParser.Main";
import { GetKvcItem } from "./FuncScriptParser.GetKvcItem";
import { GetLiteralMatch, GetLiteralMatchMultiple } from "./FuncScriptParser.GetLiteralMatch";
import { SkipSpace } from "./FuncScriptParser.SkipSpace";

export function GetSelectKvcExpression(
    context: ParseContext,
    index: number
): ParseResult {

    let i = SkipSpace(context, index).NextIndex;
    const nodeStart = i;

    // Match '{'
    const braceOpen = GetLiteralMatch(context, i, "{");
    if (braceOpen.NextIndex === i) {
        return { ParseNode: null, NextIndex: index };
    }
    i = braceOpen.NextIndex;

    // Parse body with allowKeyOnly=true, allowReturn=false, allowImplicitReturn=false
    const bodyResult = GetKvcBody(context, i, true, false, false);
    if (!bodyResult.ParseNode) {
        return { ParseNode: null, NextIndex: index };
    }
    i = SkipSpace(context, bodyResult.NextIndex).NextIndex;

    // Match '}'
    const braceClose = GetLiteralMatch(context, i, "}");
    if (braceClose.NextIndex === i) {
        context.SyntaxErrors.push(
            new SyntaxErrorData(i, 0, "'}' expected")
        );
        return { ParseNode: null, NextIndex: index };
    }
    i = braceClose.NextIndex;

    // Adjust parse node
    bodyResult.ParseNode.Pos = nodeStart;
    bodyResult.ParseNode.Length = i - nodeStart;

    return {
        ParseNode: bodyResult.ParseNode,
        NextIndex: i
    };
}

// -----------------------------------------------------------------------------

export function GetKvcExpression(
    context: ParseContext,
    index: number
): ParseResult {

    let i = SkipSpace(context, index).NextIndex;
    const nodeStart = i;

    // Match '{'
    const braceOpen = GetLiteralMatch(context, i, "{");
    if (braceOpen.NextIndex === i) {
        return { ParseNode: null, NextIndex: index };
    }
    i = braceOpen.NextIndex;

    // Parse body with allowKeyOnly=true, allowReturn=true, allowImplicitReturn=false
    const bodyResult = GetKvcBody(context, i, true, true, false);
    if (!bodyResult.ParseNode) {
        return { ParseNode: null, NextIndex: index };
    }
    i = SkipSpace(context, bodyResult.NextIndex).NextIndex;

    // Match '}'
    const braceClose = GetLiteralMatch(context, i, "}");
    if (braceClose.NextIndex === i) {
        context.SyntaxErrors.push(
            new SyntaxErrorData(i, 0, "'}' expected")
        );
        return { ParseNode: null, NextIndex: index };
    }
    i = braceClose.NextIndex;

    bodyResult.ParseNode.Pos = nodeStart;
    bodyResult.ParseNode.Length = i - nodeStart;

    return {
        ParseNode: bodyResult.ParseNode,
        NextIndex: i
    };
}

// -----------------------------------------------------------------------------

export function GetNakedKvc(
    context: ParseContext,
    index: number
): ParseResult {

    const result = GetKvcBody(context, index, false, true, true);
    if(result.NextIndex>index)
    {
        if(result.ParseNode!.Children.length==1)
        {
            return {ParseNode:result.ParseNode!.Children[0],NextIndex:result!.NextIndex};
        }
    }
    return result;
}

// -----------------------------------------------------------------------------

export function GetKvcBody(
    context: ParseContext,
    index: number,
    allowKeyOnly: boolean,
    allowReturn: boolean,
    allowImplicitReturn: boolean
): ParseResult {

    let i = SkipSpace(context, index).NextIndex;
    const children: ParseNode[] = [];
    const startPos = i;

    while (true) {
        // If we already have items, expect a comma or semicolon before next
        if (children.length > 0) {
            const delim = GetLiteralMatchMultiple(context, i, [",", ";"]);
            if (!delim.Matched) {
                break;
            }
            i = SkipSpace(context, delim.NextIndex).NextIndex;
        }

        // Parse next KVC item
        const itemRes = GetKvcItem(context, i, allowKeyOnly, allowReturn, allowImplicitReturn);
        if (!itemRes.ParseNode || itemRes.NextIndex === i) {
            break;
        }
        children.push(itemRes.ParseNode);
        i = SkipSpace(context, itemRes.NextIndex).NextIndex;
    }

    // Build the KeyValueCollection node
    const node = new ParseNode(
        ParseNodeType.KeyValueCollection,
        startPos,
        i - startPos,
        children
    );

    return {
        ParseNode: node,
        NextIndex: i
    };
}