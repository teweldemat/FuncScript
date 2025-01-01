import { ParseContext, ParseNode, ParseNodeType, ParseResult, SyntaxErrorData } from "../FuncScriptParser.Main";
import { GetLiteralMatch } from "./FuncScriptParser.GetLiteralMatch";
import { SkipSpace } from "./FuncScriptParser.SkipSpace";
import { GetIdentifier } from "./FuncScriptParser.GetIdentifier";

export function GetMemberAccess(context: ParseContext, index: number): ParseResult {
    const result = GetMemberAccessInternal(context, ".", index);
    if (result.NextIndex === index) {
        return GetMemberAccessInternal(context, "?.", index);
    }
    return result;
}

function GetMemberAccessInternal(context: ParseContext, oper: string, index: number): ParseResult {
    let parseNode: ParseNode | null = null;

    let i = SkipSpace(context, index).NextIndex;
    let i2 = GetLiteralMatch(context, i, oper).NextIndex;
    if (i2 === i) {
        return { ParseNode: null, NextIndex: index };
    }

    i = i2;
    i = SkipSpace(context, i).NextIndex;

    const identifierResult = GetIdentifier(context, i, false);

    if (identifierResult.NextIndex === i) {
        context.SyntaxErrors.push(
            new SyntaxErrorData(i, 0, "member identifier expected")
        );
        return { ParseNode: null, NextIndex: index };
    }

    i = identifierResult.NextIndex;

    parseNode = new ParseNode(
        ParseNodeType.MemberAccess,
        index,
        i - index,
        [identifierResult.ParseNode!]
    );

    return { ParseNode: parseNode, NextIndex: i };
}