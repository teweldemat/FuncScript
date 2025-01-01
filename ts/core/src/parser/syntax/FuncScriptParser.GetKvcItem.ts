import { ParseContext, ParseNode, ParseResult } from "../FuncScriptParser.Main";
import { GetIdentifier } from "./FuncScriptParser.GetIdentifier";
import { GetKeyValuePair } from "./FuncScriptParser.GetKeyValuePair";
import { GetReturnDefinition } from "./FuncScriptParser.GetReturnDefinition";
import { GetSimpleString } from "./FuncScriptParser.GetSimpleString";

export function GetKvcItem(context: ParseContext, index: number): ParseResult {
    let parseNode: ParseNode | null = null;
    
    const result = GetKeyValuePair(context, index);

    if (result.NextIndex > index) {
        return { ParseNode: result.ParseNode, NextIndex: result.NextIndex };
    }

    const returnDefResult = GetReturnDefinition(context, index);

    if (returnDefResult.NextIndex > index) {
        return { ParseNode: returnDefResult.ParseNode, NextIndex: returnDefResult.NextIndex };
    }

    // Process identifier
    const identifierResult = GetIdentifier(context, index, false);

    if (identifierResult.NextIndex > index) {
        return { ParseNode: identifierResult.ParseNode, NextIndex: identifierResult.NextIndex };
    }

    // Process simple string
    const simpleStringResult = GetSimpleString(context, index);

    if (simpleStringResult.NextIndex > index) {
        return { ParseNode: simpleStringResult.ParseNode, NextIndex: simpleStringResult.NextIndex };
    }

    return { ParseNode: null, NextIndex: index };
}