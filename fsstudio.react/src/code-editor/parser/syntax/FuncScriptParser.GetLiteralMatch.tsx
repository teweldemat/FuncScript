import { ParseContext } from "../FuncScriptParser.Main";

export function GetLiteralMatch(context: ParseContext, index: number, ...candidates: string[]): GetLiteralMatchResult {
    return GetLiteralMatchFromString(context.Expression, index, ...candidates);
}

export function GetLiteralMatchFromString(expression: string, index: number, ...candidates: string[]): GetLiteralMatchResult {
    if (expression === null) {
        throw new Error("The input expression cannot be null.");
    }

    let matched: string | null = null;
    for (const k of candidates) {
        let matchFound = true;
        if (index + k.length <= expression.length) {
            for (let i = 0; i < k.length; i++) {
                if (expression[index + i].toLowerCase() !== k[i].toLowerCase()) {
                    matchFound = false;
                    break;
                }
            }

            if (matchFound) {
                matched = k.toLowerCase();
                return { Matched: matched, NextIndex: index + k.length };
            }
        }
    }

    return { Matched: matched, NextIndex: index };
}

interface GetLiteralMatchResult {
    Matched: string | null;
    NextIndex: number;
}