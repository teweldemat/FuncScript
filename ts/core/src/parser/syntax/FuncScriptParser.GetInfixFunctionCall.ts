import { ParseContext, ParseNode, ParseNodeType, SyntaxErrorData } from "../FuncScriptParser.Main";
import { GetCallAndMemberAccess } from "./FuncScriptParser.GetCallAndMemberAccess";
import { GetIdentifier } from "./FuncScriptParser.GetIdentifier";
import { GetLiteralMatch } from "./FuncScriptParser.GetLiteralMatch";
import { SkipSpace } from "./FuncScriptParser.SkipSpace";

export function GetInfixFunctionCall(context: ParseContext, index: number): { ParseNode: ParseNode | null, NextIndex: number } {
    const originalIndex = index; 
    const childNodes: ParseNode[] = [];

    const leftResult = GetCallAndMemberAccess(context, index);
    if (leftResult.NextIndex === index) {
        return { ParseNode: null, NextIndex: index };
    }

    let parseNode = leftResult.ParseNode;
    childNodes.push(parseNode!);

    let i = SkipSpace(context, leftResult.NextIndex).NextIndex;
    let foundAnyOperator = false;

    while (true) {
        const operatorResult = GetIdentifier(context, i, true);
        if (operatorResult.NextIndex === i) {
            break; 
        }


        foundAnyOperator = true; 
        childNodes.push(operatorResult.ParseNode!);
        i = SkipSpace(context, operatorResult.NextIndex).NextIndex;

        const rightResult = GetCallAndMemberAccess(context, i);
        if (rightResult.NextIndex === i) {
            context.SyntaxErrors.push(new SyntaxErrorData(
                i, 
                0, 
                `Right side operand expected for ${operatorResult.Iden}`
            ));
            return { ParseNode: null, NextIndex: originalIndex };
        }

        childNodes.push(rightResult.ParseNode!);
        i = SkipSpace(context, rightResult.NextIndex).NextIndex;

        while (true) {
            const literalMatchResult = GetLiteralMatch(context, i, "~");
            if (literalMatchResult.NextIndex === i) break;

            i = SkipSpace(context, literalMatchResult.NextIndex).NextIndex;
            const moreOperandResult = GetCallAndMemberAccess(context, i);
            if (moreOperandResult.NextIndex === i) break;

            i = SkipSpace(context, moreOperandResult.NextIndex).NextIndex;
            childNodes.push(moreOperandResult.ParseNode!);
        }
    }

    if (!foundAnyOperator) {
        return leftResult; 
    }

    parseNode = new ParseNode(
        ParseNodeType.InfixExpression, 
        childNodes[0].Pos, 
        childNodes[childNodes.length - 1].Pos + childNodes[childNodes.length - 1].Length - childNodes[0].Pos, 
        childNodes
    );

    return { ParseNode: parseNode, NextIndex: i };
}