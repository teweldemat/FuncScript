import { KW_CASE, ParseContext, ParseNode, ParseNodeType, ParseResult, SyntaxErrorData } from "../FuncScriptParser.Main"; 
import { GetLiteralMatch, GetLiteralMatchMultiple } from "./FuncScriptParser.GetLiteralMatch"; 
import { SkipSpace } from "./FuncScriptParser.SkipSpace"; 
import { GetExpression } from "./FuncScriptParser.GetExpression"; 

export function GetCaseExpression(context: ParseContext, index: number): ParseResult { 
    let parseNode: ParseNode | null = null; 
    let i = index; 
    let literalMatchResult = GetLiteralMatch(context, i, KW_CASE); 

    if (literalMatchResult.NextIndex === i) { 
        return { ParseNode: null, NextIndex: index }; 
    } 

    i = SkipSpace(context, literalMatchResult.NextIndex).NextIndex; 
    const childNodes: ParseNode[] = []; 
    do { 
        if (childNodes.length === 0) { 
            const expressionResult = GetExpression(context, i); 
            if (expressionResult.NextIndex === i) { 
                context.SyntaxErrors.push(new SyntaxErrorData(i, 1, "Case condition expected")); 
                return { ParseNode: null, NextIndex: index }; 
            } 

            childNodes.push(expressionResult.ParseNode!); 
            i = SkipSpace(context, expressionResult.NextIndex).NextIndex; 
        } else { 
            literalMatchResult = GetLiteralMatchMultiple(context, i, [",", ";"]); 
            if (literalMatchResult.NextIndex === i) { 
                break; 
            } 
            i = SkipSpace(context, literalMatchResult.NextIndex).NextIndex; 
            const expressionResult = GetExpression(context, i); 
            if (expressionResult.NextIndex === i) { 
                break; 
            } 
            childNodes.push(expressionResult.ParseNode!); 
            i = SkipSpace(context, expressionResult.NextIndex).NextIndex; 
        } 

        literalMatchResult = GetLiteralMatch(context, i, ":"); 
        if (literalMatchResult.NextIndex === i) { 
            break; 
        } 

        i = SkipSpace(context, literalMatchResult.NextIndex).NextIndex; 
        const valueExpressionResult = GetExpression(context, i); 
        if (valueExpressionResult.NextIndex === i) { 
            context.SyntaxErrors.push(new SyntaxErrorData(i, 1, "Case value expected")); 
            return { ParseNode: null, NextIndex: index }; 
        } 

        childNodes.push(valueExpressionResult.ParseNode!); 
        i = SkipSpace(context, valueExpressionResult.NextIndex).NextIndex; 
    } while (true); 

    parseNode = new ParseNode(ParseNodeType.Case, index, i - index, childNodes); 
    return { ParseNode: parseNode, NextIndex: i }; 
}