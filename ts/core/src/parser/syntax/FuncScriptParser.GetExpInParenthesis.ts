import { ParseContext, ParseNode, ParseNodeType, ParseResult, SyntaxErrorData } from "../FuncScriptParser.Main";
import { GetExpression } from "./FuncScriptParser.GetExpression";
import { GetLiteralMatch } from "./FuncScriptParser.GetLiteralMatch";
import { SkipSpace } from "./FuncScriptParser.SkipSpace";

export function GetExpInParenthesis(context: ParseContext, index: number): ParseResult {
    let parseNode: ParseNode | null = null;
    let i = index;
    i = SkipSpace(context, i).NextIndex;
    let i2 = GetLiteralMatch(context, i, "(").NextIndex;
    if (i === i2) {
        return {  ParseNode: parseNode, NextIndex: index };
    }
    i = i2;

    i = SkipSpace(context, i).NextIndex;
    const expressionResult = GetExpression(context, i);
    const nodeExpression = expressionResult.ParseNode;
    if (expressionResult.NextIndex> i) {
        i = expressionResult.NextIndex;
    }
    i = SkipSpace(context, i).NextIndex;
    i2 = GetLiteralMatch(context, i, ")").NextIndex;
    if (i === i2) {
        context.SyntaxErrors.push(new SyntaxErrorData(i, 0, "')' expected"));
        return {  ParseNode: parseNode, NextIndex: index };
    }

    i = i2;

    parseNode = new ParseNode(ParseNodeType.ExpressionInBrace, index, i - index, [nodeExpression!]);
    return {  ParseNode: parseNode, NextIndex: i };
}