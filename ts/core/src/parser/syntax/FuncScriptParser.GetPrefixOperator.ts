import { ParseContext, ParseNode, ParseNodeType, s_prefixOp, SyntaxErrorData } from "../FuncScriptParser.Main";
import { GetCallAndMemberAccess } from "./FuncScriptParser.GetCallAndMemberAccess";
import { GetLiteralMatch } from "./FuncScriptParser.GetLiteralMatch";
import { GetOperator } from "./FuncScriptParser.GetOperator";
import { SkipSpace } from "./FuncScriptParser.SkipSpace";

export function GetPrefixOperator(context: ParseContext, index: number) {
    let i = 0;
    let oper: string | null = null;
    let opNode: ParseNode | null = null;

    const matchResult = GetOperator(context,  s_prefixOp.map(x=>x[0]),index);
    i=matchResult.NextIndex
    oper=matchResult.MatchedOp
    if (i === index) {
        return { ParseNode: null, NextIndex: index };
    }

    if (!oper) {
        context.SyntaxErrors.push(new SyntaxErrorData(index, i - index, `Prefix operator ${oper} not defined`));
        return { ParseNode: null, NextIndex: index };
    }
    i = SkipSpace(context, i).NextIndex;

    const operandRes = GetCallAndMemberAccess(context, i);
    if (operandRes.NextIndex === i) {
        context.SyntaxErrors.push(new SyntaxErrorData(i, 0, `Operand for ${oper} expected`));
        return { ParseNode: null, NextIndex: index };
    }

    i = SkipSpace(context, operandRes.NextIndex).NextIndex;

    const parseNode = new ParseNode(
        ParseNodeType.PrefixOperatorExpression,
        index,
        i - index,
        [matchResult.ParseNode!, operandRes.ParseNode!]
    );

    return { ParseNode: parseNode, NextIndex: i };
}