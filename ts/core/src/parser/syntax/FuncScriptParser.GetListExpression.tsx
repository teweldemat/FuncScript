import { ParseContext, ParseNode, ParseNodeType, SyntaxErrorData } from '../FuncScriptParser.Main';
import { GetExpression } from './FuncScriptParser.GetExpression';
import { GetLiteralMatch } from './FuncScriptParser.GetLiteralMatch';
import { SkipSpace } from './FuncScriptParser.SkipSpace';

export const  GetListExpression = (context: ParseContext, index: number): any => {
    
    let i = SkipSpace(context, index).NextIndex;
    let i2 = GetLiteralMatch(context, i, "[").NextIndex;
    if (i2 === i) return { ParseNode: null, NextIndex: index };
    
    let tokenStart = i2;
    i = i2;
    
    let nodeListItems: ParseNode[] = [];
    i = SkipSpace(context, i).NextIndex;
    let expressionResult = GetExpression(context, i);
    if (expressionResult.NextIndex > i) {
        nodeListItems.push(expressionResult.ParseNode!);
        i = expressionResult.NextIndex;
        do {
            i = SkipSpace(context, i).NextIndex;
            i2 = GetLiteralMatch(context, i, ",").NextIndex;
            if (i2 === i) break;
            i = i2;

            i = SkipSpace(context, i).NextIndex;
            expressionResult = GetExpression(context, i);
            if (expressionResult.NextIndex === i) break;
            nodeListItems.push(expressionResult.ParseNode!);
            i = expressionResult.NextIndex;
        } while (true);
    }
    
    i = SkipSpace(context, i).NextIndex;
    i2 = GetLiteralMatch(context, i, "]").NextIndex;
    if (i2 === i) {
        context.SyntaxErrors.push(new SyntaxErrorData(i, 0, "']' expected"));
        return { ParseNode: null, NextIndex: index };
    }

    i = i2;
    let parseNode = new ParseNode(ParseNodeType.List, index, i - tokenStart, nodeListItems);
    return { ParseNode: parseNode, NextIndex: i };
};

