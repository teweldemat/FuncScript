import { ParseContext, ParseNode, ParseNodeType, SyntaxErrorData } from "../FuncScriptParser.Main";
import { isCharWhiteSpace } from "../FuncScriptParser.Utils";
import { GetCommentBlock } from "./FuncScriptParser.GetCommentBlock";

export function SkipSpace(context: ParseContext, index: number): { NextIndex: number } {
    let i = index;
    const expression = context.Expression;
    
    while (index < expression.length) {
        if (isCharWhiteSpace(expression[index])) {
            index++;
        } else {
            const commentBlockResult = GetCommentBlock(context, index);
            if (commentBlockResult.NextIndex === index) {
                break;
            }
            index = commentBlockResult.NextIndex;
        }
    }

    return { NextIndex: index };
}