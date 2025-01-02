import { ParseContext, ParseNode, ParseNodeType, ParseResult, SyntaxErrorData } from "../FuncScriptParser.Main"; 
import { GetKeyValuePair } from "./FuncScriptParser.GetKeyValuePair"; 
import { GetReturnDefinition } from "./FuncScriptParser.GetReturnDefinition"; 
import { GetIdentifier } from "./FuncScriptParser.GetIdentifier"; 
import { GetSimpleString } from "./FuncScriptParser.GetSimpleString"; 

interface KeyValueExpression { 
    Key: string | null; 
    KeyLower?: string; 
    ValueExpression: any; 
} 

 

export function GetKvcItem(context: ParseContext, index: number, allowKeyOnly: boolean): ParseResult { 
    let item: KeyValueExpression | null = null; 
    const result = GetKeyValuePair(context, index, allowKeyOnly); 

    if (result.NextIndex > index) { 
        return result;
    } 

    const returnDefResult = GetReturnDefinition(context, index); 

    if (returnDefResult.NextIndex > index) { 
        return { ParseNode: returnDefResult.ParseNode, NextIndex: returnDefResult.NextIndex }; 
    } 

    if (!allowKeyOnly) { 
        return { ParseNode: null, NextIndex: index }; 
    } 

    const identifierResult = GetIdentifier(context, index, false); 

    if (identifierResult.NextIndex > index) { 
        return { ParseNode: identifierResult.ParseNode, NextIndex: identifierResult.NextIndex }; 
    } 

    const simpleStringResult = GetSimpleString(context, index); 

    if (simpleStringResult.NextIndex > index) { 
        return { ParseNode: simpleStringResult.ParseNode, NextIndex: simpleStringResult.NextIndex }; 
    } 

    return {  ParseNode: null, NextIndex: index }; 
}