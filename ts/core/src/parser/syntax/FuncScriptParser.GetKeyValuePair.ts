import { ParseContext, ParseNode, ParseNodeType, SyntaxErrorData, ParseResult } from "../FuncScriptParser.Main";
import { GetExpression } from "./FuncScriptParser.GetExpression";
import { GetIdentifier } from "./FuncScriptParser.GetIdentifier";
import { GetLiteralMatch } from "./FuncScriptParser.GetLiteralMatch";
import { GetSimpleString } from "./FuncScriptParser.GetSimpleString";
import { SkipSpace } from "./FuncScriptParser.SkipSpace";

export function GetKeyValuePair(context: ParseContext, index: number): ParseResult {
    let parseNode: ParseNode | null = null;
    let i = index;
    let name: string|null;
    let nameLower: string|null;
    let nodeName: ParseNode|null;
    
    ({ Str: name, ParseNode: nodeName, NextIndex: i } = GetSimpleString(context, index));
    
    if (i === index) {
        ({ Iden: name, IdenLower: nameLower,  ParseNode: nodeName, NextIndex: i } = GetIdentifier(context, index, false));
        
        if (i === index) {
            return { ParseNode: null, NextIndex: index };
        }
    } else {
        nameLower = name!.toLowerCase();
    }

    i = SkipSpace(context, i).NextIndex;

    let i2 = GetLiteralMatch(context, i, ":").NextIndex;
    if (i2 === i) {
        nodeName!.NodeType = ParseNodeType.Key;
        parseNode = new ParseNode(ParseNodeType.KeyValuePair, index, i - index, [nodeName!]);
        return { ParseNode: parseNode, NextIndex: i };
    }

    i = i2;
    i = SkipSpace(context, i).NextIndex;
     
    var nodeExpBlock:ParseNode|null;
    ({ ParseNode: nodeExpBlock, NextIndex: i2 } = GetExpression(context, i));
    
    if (i2 === i) {
        context.SyntaxErrors.push(new SyntaxErrorData(i, 0, "value expression expected"));
        return { ParseNode: null, NextIndex: index };
    }

    i = i2;
    i = SkipSpace(context, i).NextIndex;
    nodeName!.NodeType = ParseNodeType.Key;
    parseNode = new ParseNode(ParseNodeType.KeyValuePair, index, i - index, [nodeName!, nodeExpBlock!]);
    return { ParseNode: parseNode, NextIndex: i };
}