import { KW_IF, KW_THEN, KW_ELSE, ParseContext, ParseNode, ParseNodeType, ParseResult, SyntaxErrorData } from "../FuncScriptParser.Main";
import { GetLiteralMatch } from "./FuncScriptParser.GetLiteralMatch";
import { SkipSpace } from "./FuncScriptParser.SkipSpace";
import { GetExpression } from "./FuncScriptParser.GetExpression";

export function GetIfExpression(context: ParseContext, index: number): ParseResult {
    let parseNode: ParseNode | null = null;
    let i = index;

    let literalMatchResult = GetLiteralMatch(context, i, KW_IF);
    if (literalMatchResult.NextIndex === i) {
        return { ParseNode: null, NextIndex: index };
    }
    const childNodes: ParseNode[] = [];
    
    childNodes.push(new ParseNode(ParseNodeType.KeyWord,i, literalMatchResult.NextIndex-i));

    i = SkipSpace(context, literalMatchResult.NextIndex).NextIndex;

    const conditionResult = GetExpression(context, i);
    if (conditionResult.NextIndex === i) {
        context.SyntaxErrors.push(new SyntaxErrorData(i, 1, "If condition expected"));
        return { ParseNode: null, NextIndex: index };
    }
    childNodes.push(conditionResult.ParseNode!);

    i = SkipSpace(context, conditionResult.NextIndex).NextIndex;

    literalMatchResult = GetLiteralMatch(context, i, KW_THEN);
    if (literalMatchResult.NextIndex === i) {
        context.SyntaxErrors.push(new SyntaxErrorData(i, 1, "Keyword 'then' expected"));
        return { ParseNode: null, NextIndex: index };
    }
    childNodes.push(new ParseNode(ParseNodeType.KeyWord,i, literalMatchResult.NextIndex-i));

    i = SkipSpace(context, literalMatchResult.NextIndex).NextIndex;

    const trueExprResult = GetExpression(context, i);
    if (trueExprResult.NextIndex === i) {
        context.SyntaxErrors.push(new SyntaxErrorData(i, 1, "Then expression expected"));
        return { ParseNode: null, NextIndex: index };
    }

    childNodes.push(trueExprResult.ParseNode!);
    i = SkipSpace(context, trueExprResult.NextIndex).NextIndex;

    literalMatchResult = GetLiteralMatch(context, i, KW_ELSE);
    if (literalMatchResult.NextIndex !== i) {
        childNodes.push(new ParseNode(ParseNodeType.KeyWord,i, literalMatchResult.NextIndex-i));
        i = SkipSpace(context, literalMatchResult.NextIndex).NextIndex;

        const elseExprResult = GetExpression(context, i);
        if (elseExprResult.NextIndex === i) {
            context.SyntaxErrors.push(new SyntaxErrorData(i, 1, "Else expression expected"));
            return { ParseNode: null, NextIndex: index };
        }

        childNodes.push(elseExprResult.ParseNode!);
        i = SkipSpace(context, elseExprResult.NextIndex).NextIndex;
    }

    parseNode = new ParseNode(ParseNodeType.IfExpression, index, i - index, childNodes);
    return { ParseNode: parseNode, NextIndex: i };
} 