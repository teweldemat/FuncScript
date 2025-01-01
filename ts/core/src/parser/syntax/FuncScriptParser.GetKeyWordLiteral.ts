import { ParseContext, ParseNode, ParseNodeType, ParseResult } from "../FuncScriptParser.Main";
import { GetLiteralMatch } from "./FuncScriptParser.GetLiteralMatch";

export function GetKeyWordLiteral(context: ParseContext, index: number): ParseResult {
    let parseNode: ParseNode | null = null;
    let i = GetLiteralMatch(context, index, "null").NextIndex;
    let literal: any;

    if (i > index) {
        literal = null;
    } else if ((i = GetLiteralMatch(context, index, "true").NextIndex) > index) {
        literal = true;
    } else if ((i = GetLiteralMatch(context, index, "false").NextIndex) > index) {
        literal = false;
    } else {
        return { ParseNode: null, NextIndex: index };
    }

    parseNode = new ParseNode(ParseNodeType.KeyWord, index, i - index);
    return { ParseNode: parseNode, NextIndex: i };
}