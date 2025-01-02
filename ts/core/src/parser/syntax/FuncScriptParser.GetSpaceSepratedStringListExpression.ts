import { ParseContext, ParseNode, ParseNodeType, ParseResult, SyntaxErrorData } from "../FuncScriptParser.Main"; 
import { GetLiteralMatch } from "./FuncScriptParser.GetLiteralMatch"; 
import { SkipSpace } from "./FuncScriptParser.SkipSpace"; 
import { GetSimpleString } from "./FuncScriptParser.GetSimpleString"; 
import { GetSpaceLessString } from "./FuncScriptParser.GetSpaceLessString"; 

 

export function GetSpaceSeparatedStringListExpression(context: ParseContext, index: number): ParseResult { 

    return {ParseNode: null,NextIndex:index }; 
}