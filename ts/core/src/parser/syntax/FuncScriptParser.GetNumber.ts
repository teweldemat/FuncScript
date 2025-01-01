import { ParseContext, ParseNode, ParseNodeType, ParseResult, SyntaxErrorData } from "../FuncScriptParser.Main";
import { GetInt } from "./FuncScriptParser.GetInt";
import { GetLiteralMatch } from "./FuncScriptParser.GetLiteralMatch";

export function GetNumber(context: ParseContext, index: number): ParseResult{
    let parseNode: ParseNode | null = null;
    let hasDecimal = false;
    let hasExp = false;
    let hasLong = false;
    let i = index;
    
    const intResult = GetInt(context, true, i);
    let {  ParseNode: nodeDigits, NextIndex: i2 } = intResult;
    if (i2 === i) {
        return { ParseNode: null, NextIndex: index };
    }
    i = i2;

    i2 = GetLiteralMatch(context, i, ".").NextIndex;
    if (i2 > i) {
        hasDecimal = true;
    }
    i = i2;
    if (hasDecimal) {
        const decimalResult = GetInt(context, false, i);
        i = decimalResult.NextIndex;
    }

    i2 = GetLiteralMatch(context, i, "E").NextIndex;
    if (i2 > i) {
        hasExp = true;
    }
    i = i2;

    let expDigits: string | null = null;
    let nodeExpDigits: ParseNode;
    if (hasExp) {
        const expResult = GetInt(context, true, i);
        nodeExpDigits = expResult.ParseNode!;
        i = expResult.NextIndex;
    }

    if (!hasDecimal) {
        i2 = GetLiteralMatch(context, i, "l").NextIndex;
        if (i2 > i) {
            hasLong = true;
        }
        i = i2;
    }

    if (hasDecimal) {
        parseNode = new ParseNode(ParseNodeType.LiteralDouble, index, i - index);
        return { ParseNode: parseNode, NextIndex: i };
    }

    let longVal: number;

    if (hasLong) {
        parseNode = new ParseNode(ParseNodeType.LiteralLong, index, i - index);
        return { ParseNode: parseNode, NextIndex: i };
    }

    parseNode = new ParseNode(ParseNodeType.LiteralInteger, index, i - index);
    return { ParseNode: parseNode, NextIndex: i };

}