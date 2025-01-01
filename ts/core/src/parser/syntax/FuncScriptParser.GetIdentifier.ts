import { ParseContext, ParseNode, ParseNodeType, ParseResult, s_KeyWords } from "../FuncScriptParser.Main";
import { GetLiteralMatch } from "./FuncScriptParser.GetLiteralMatch";
import { IsIdentifierFirstChar } from "./FuncScriptParser.IsIdentfierFirstChar";
import { IsIdentfierOtherChar } from "./FuncScriptParser.IsIdentfierOtherChar";

export interface GetIdentifierResult extends ParseResult {
    Iden: string | null;
    IdenLower: string | null;
    ParentRef: boolean;
}

export function GetIdentifier(context: ParseContext, index: number, supportParentRef: boolean): GetIdentifierResult {
    let iden: string | null = null;
    let idenLower: string | null = null;
    let parentRef = false;
    let parseNode: ParseNode | null = null;

    const exp = context.Expression;

    if (index >= exp.length) {
        return { Iden: iden, IdenLower: idenLower, ParentRef: parentRef, ParseNode: null, NextIndex: index };
    }

    let i1 = index;
    let i = i1;

    if (supportParentRef) {
        const i2 = GetLiteralMatch(context, i, "^").NextIndex;
        if (i2 > i) {
            parentRef = true;
            i1 = i2;
            i = i2;
        }
    }

    if (!IsIdentifierFirstChar(exp[i])) {
        return { Iden: iden, IdenLower: idenLower, ParentRef: parentRef, ParseNode: null, NextIndex: index };
    }

    i++;
    while (i < exp.length && IsIdentfierOtherChar(exp[i])) {
        i++;
    }

    iden = exp.substring(i1, i);
    idenLower = iden.toLowerCase();
    if (s_KeyWords.has(idenLower)) {
        return { Iden: iden, IdenLower: idenLower, ParentRef: parentRef, ParseNode: null, NextIndex: index };
    }

    parseNode = new ParseNode(ParseNodeType.Identifier, index, i - index);
    return { Iden: iden, IdenLower: idenLower, ParentRef: parentRef, ParseNode: parseNode, NextIndex: i };
}