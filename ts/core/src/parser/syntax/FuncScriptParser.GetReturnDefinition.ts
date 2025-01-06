import { 
    KW_RETURN, 
    ParseContext, 
    ParseNode, 
    ParseNodeType, 
    ParseResult, 
    SyntaxErrorData 
} from "../FuncScriptParser.Main";
import { GetExpression } from "./FuncScriptParser.GetExpression";
import { GetLiteralMatch } from "./FuncScriptParser.GetLiteralMatch";
import { SkipSpace } from "./FuncScriptParser.SkipSpace";

export function GetReturnDefinition(
    context: ParseContext, 
    index: number,
    allowImplicitReturn: boolean
): ParseResult {
    let parseNode: ParseNode | null = null;
    
    let i = GetLiteralMatch(context, index, KW_RETURN).NextIndex;
    if (i === index) {
        if (allowImplicitReturn) {
            return GetExpression(context, index);
        }
        return { ParseNode: null, NextIndex: index };
    }

    const nodeReturn = new ParseNode(ParseNodeType.KeyWord, index, i - index);
    i = SkipSpace(context, i).NextIndex;

    const exprResult = GetExpression(context, i);
    if (exprResult.NextIndex === i) {
        context.SyntaxErrors.push(new SyntaxErrorData(i, 0, "return expression expected"));
        return { ParseNode: null, NextIndex: index };
    }

    i = exprResult.NextIndex;
    parseNode = new ParseNode(
        ParseNodeType.ReturnExpression, 
        index, 
        i - index, 
        [nodeReturn, exprResult.ParseNode!]
    );

    return { ParseNode: parseNode, NextIndex: i };
}