"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.SkipSpace = SkipSpace;
const FuncScriptParser_Utils_1 = require("../FuncScriptParser.Utils");
const FuncScriptParser_GetCommentBlock_1 = require("./FuncScriptParser.GetCommentBlock");
function SkipSpace(context, index) {
    let i = index;
    const expression = context.Expression;
    while (index < expression.length) {
        if ((0, FuncScriptParser_Utils_1.isCharWhiteSpace)(expression[index])) {
            index++;
        }
        else {
            const commentBlockResult = (0, FuncScriptParser_GetCommentBlock_1.GetCommentBlock)(context, index);
            if (commentBlockResult.NextIndex === index) {
                break;
            }
            index = commentBlockResult.NextIndex;
        }
    }
    return { NextIndex: index };
}
