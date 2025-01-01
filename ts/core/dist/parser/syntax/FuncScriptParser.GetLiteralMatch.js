"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.GetLiteralMatch = GetLiteralMatch;
exports.GetLiteralMatchMultiple = GetLiteralMatchMultiple;
exports.GetLiteralMatchFromString = GetLiteralMatchFromString;
function GetLiteralMatch(context, index, candidate) {
    return GetLiteralMatchFromString(context.Expression, index, [candidate]);
}
function GetLiteralMatchMultiple(context, index, candidates) {
    return GetLiteralMatchFromString(context.Expression, index, candidates);
}
function GetLiteralMatchFromString(expression, index, candidates) {
    if (expression === null) {
        throw new Error("The input expression cannot be null.");
    }
    let matched = null;
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
