import { ParseContext, ParseNode, ParseNodeType, ParseResult, SyntaxErrorData } from "../FuncScriptParser.Main"; 
import { GetLiteralMatch } from "./FuncScriptParser.GetLiteralMatch"; 
import { SkipSpace } from "./FuncScriptParser.SkipSpace"; 
import { GetSimpleString } from "./FuncScriptParser.GetSimpleString"; 
import { GetSpaceLessString } from "./FuncScriptParser.GetSpaceLessString"; 

interface GetSpaceSeparatedStringListExpressionResult extends ParseResult { 
    StringList: string[]; 
} 

export function GetSpaceSeparatedStringListExpression(context: ParseContext, index: number): GetSpaceSeparatedStringListExpressionResult { 
    let i = SkipSpace(context, index).NextIndex; 
    let listItems: string[] = []; 
    let nodeListItems: ParseNode[] = []; 

    let otherItem: string; 
    let otherNode: ParseNode | null; 

    const firstResult = GetSimpleString(context, i); 
    let firstItem = firstResult.String; 
    let firstNode = firstResult.ParseNode; 
    let i2 = firstResult.NextIndex; 

    if (i2 === i) { 
        const spaceLessResult = GetSpaceLessString(context, i); 
        firstItem = spaceLessResult.String; 
        firstNode = spaceLessResult.ParseNode; 
        i2 = spaceLessResult.NextIndex; 
    } 

    if (i2 > i) { 
        listItems.push(firstItem); 
        nodeListItems.push(firstNode!); 
        i = i2; 
        
        do { 
            i2 = GetLiteralMatch(context, i, " ").NextIndex; 
            if (i2 === i) break; 

            i = i2; 
            i = SkipSpace(context, i).NextIndex; 

            const otherResult = GetSimpleString(context, i); 
            otherItem = otherResult.String; 
            otherNode = otherResult.ParseNode; 
            i2 = otherResult.NextIndex; 

            if (i2 === i) { 
                const spaceLessResult = GetSpaceLessString(context, i); 
                otherItem = spaceLessResult.String; 
                otherNode = spaceLessResult.ParseNode; 
                i2 = spaceLessResult.NextIndex; 
            } 

            if (i2 === i) break; 

            listItems.push(otherItem); 
            nodeListItems.push(otherNode!); 
            i = i2; 
        } while (true); 
    } 

    const parseNode = new ParseNode(ParseNodeType.List, index, i - index, nodeListItems); 
    return { StringList: listItems, ParseNode: parseNode, NextIndex: i }; 
}