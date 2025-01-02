import { ParseContext, ParseNode, ParseNodeType, ParseResult } from "../FuncScriptParser.Main";  
import { GetLiteralMatchMultiple } from "./FuncScriptParser.GetLiteralMatch";

export interface GetCommentBlockResult extends ParseResult {}  

export function GetCommentBlock(context: ParseContext, index: number): GetCommentBlockResult {  
    let parseNode: ParseNode | null = null;  
    let i = GetLiteralMatchMultiple(context, index, ["//"]).NextIndex;  

    if (i === index) {  
        return { ParseNode: parseNode, NextIndex: index };  
    }  

    const i2 = context.Expression.indexOf("\n", i);  

    if (i2 === -1) {  
        i = context.Expression.length;  
    } else {  
        i = i2 + 1;  
    }  

    parseNode = new ParseNode(ParseNodeType.Comment, index, i - index);  

    return { ParseNode: parseNode, NextIndex: i };  
}  