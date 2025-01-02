import {
    ParseContext,
    ParseNode,
    ParseNodeType,
    SyntaxErrorData,
    ParseResult
} from "../FuncScriptParser.Main";
import { GetExpression } from "./FuncScriptParser.GetExpression";
import { GetIdentifier } from "./FuncScriptParser.GetIdentifier";
import { GetLiteralMatch } from "./FuncScriptParser.GetLiteralMatch";
import { GetSimpleString } from "./FuncScriptParser.GetSimpleString";
import { SkipSpace } from "./FuncScriptParser.SkipSpace";

export function GetKeyValuePair(context: ParseContext, index: number, allowKeyOnly: boolean): ParseResult {
    let parseNode: ParseNode | null = null;
    let i = index;
    let name: string | null;
    let nameLower: string | null;
    let nodeName: ParseNode | null;
    
    // Attempt to get a simple string
    ({ Str: name, ParseNode: nodeName, NextIndex: i } = GetSimpleString(context, index));
    
    // If simple string retrieval fails, try identifier
    if (i === index) {
        ({ Iden: name, IdenLower: nameLower, ParseNode: nodeName, NextIndex: i } = GetIdentifier(context, index, false));
        
        // If both retrievals fail, return immediately
        if (i === index) {
            return { ParseNode: null, NextIndex: index };
        }
    } else {
        // Convert name to lower case for further processing
        nameLower = name!.toLowerCase();
    }
    
    // Skip spaces
    i = SkipSpace(context, i).NextIndex;

    // Check for the ':' character
    let i2 = GetLiteralMatch(context, i, ":").NextIndex;
    if (i2 === i) {
        // If ':' is not found and key-only is not allowed, raise a syntax error
        if (!allowKeyOnly) {
            context.SyntaxErrors.push(new SyntaxErrorData(i, 0, "':' expected"));
            return { ParseNode: null, NextIndex: index };
        }

        // Create a key-only node if allowed
        nodeName!.NodeType = ParseNodeType.Key;
        parseNode = new ParseNode(ParseNodeType.KeyValuePair, index, i - index, [nodeName!]);
        return { ParseNode: parseNode, NextIndex: i };
    }

    // Update index if ':' is found
    i = i2;
    
    // Skip spaces before the expression
    i = SkipSpace(context, i).NextIndex;

    // Attempt to get the expression
    let nodeExpBlock: ParseNode | null;
    ({ ParseNode: nodeExpBlock, NextIndex: i2 } = GetExpression(context, i));
    
    // If expression retrieval fails, raise a syntax error
    if (i2 === i) {
        context.SyntaxErrors.push(new SyntaxErrorData(i, 0, "value expression expected"));
        return { ParseNode: null, NextIndex: index };
    }

    // Update index after successful expression retrieval
    i = i2;

    // Skip any trailing spaces
    i = SkipSpace(context, i).NextIndex;

    // Assign as key type and create the key-value node
    nodeName!.NodeType = ParseNodeType.Key;
    parseNode = new ParseNode(ParseNodeType.KeyValuePair, index, i - index, [nodeName!, nodeExpBlock!]);
    return { ParseNode: parseNode, NextIndex: i };
}