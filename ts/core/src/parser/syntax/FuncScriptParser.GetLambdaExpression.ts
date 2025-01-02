import { ParseContext, ParseNode, ParseNodeType, ParseResult, SyntaxErrorData } from "../FuncScriptParser.Main"; 
import { GetLiteralMatch } from "./FuncScriptParser.GetLiteralMatch"; 
import { SkipSpace } from "./FuncScriptParser.SkipSpace"; 
import { GetExpression } from "./FuncScriptParser.GetExpression"; 
import { GetIdentifierList } from "./FuncScriptParser.GetIdentifierList"; 

export function GetLambdaExpression(context: ParseContext, index: number): ParseResult { 
    let parseNode: ParseNode | null = null; 

    const { ParseNode: nodesParams, NextIndex: i_initial } = GetIdentifierList(context, index); 
    let i = i_initial; 
    if (i === index) { 
        return { ParseNode: parseNode, NextIndex: index }; 
    } 

    i = SkipSpace(context, i).NextIndex; 
    if (i >= context.Expression.length - 1) { 
        return { ParseNode: parseNode, NextIndex: index }; 
    } 

    const i2 = GetLiteralMatch(context, i, "=>").NextIndex; 
    if (i2 === i) { 
        context.SyntaxErrors.push(new SyntaxErrorData(i, 0, "'=>' expected")); 
        return { ParseNode: parseNode, NextIndex: index }; 
    } 

    i += 2; 
    i = SkipSpace(context, i).NextIndex; 

    const { ParseNode: nodeDefination, NextIndex: i_final } = GetExpression(context, i); 
    if (i_final === i) { 
        context.SyntaxErrors.push(new SyntaxErrorData(i, 0, "definition of lambda expression expected")); 
        return { ParseNode: parseNode, NextIndex: index }; 
    } 

    i = i_final; 
    parseNode = new ParseNode(ParseNodeType.LambdaExpression, index, i - index, [nodesParams!, nodeDefination!]); 
    return { ParseNode: parseNode, NextIndex: i }; 
}