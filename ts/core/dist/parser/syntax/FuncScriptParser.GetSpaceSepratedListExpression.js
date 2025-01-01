"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const FuncScriptParser_Main_1 = require("../FuncScriptParser.Main");
const FuncScriptParser_GetExpression_1 = require("./FuncScriptParser.GetExpression");
const FuncScriptParser_GetLiteralMatch_1 = require("./FuncScriptParser.GetLiteralMatch");
const FuncScriptParser_SkipSpace_1 = require("./FuncScriptParser.SkipSpace");
function GetSpaceSepratedListExpression(context, index) {
    let parseNode = null;
    let i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, index).NextIndex;
    const tokenStart = i;
    const nodeListItems = [];
    let nodeFirstItem = null;
    let i2;
    // Use parentheses for destructuring and ensure variables can be reassigned
    ({ ParseNode: nodeFirstItem, NextIndex: i2 } = (0, FuncScriptParser_GetExpression_1.GetExpression)(context, i));
    if (i2 > i) {
        nodeListItems.push(nodeFirstItem);
        i = i2;
        do {
            i2 = (0, FuncScriptParser_GetLiteralMatch_1.GetLiteralMatch)(context, i, " ").NextIndex;
            if (i2 === i)
                break;
            i = i2;
            i = (0, FuncScriptParser_SkipSpace_1.SkipSpace)(context, i).NextIndex;
            // Destructure properly or adjust depending on the function's return
            const { ParseNode: nodeOtherItem, NextIndex: newIndex } = (0, FuncScriptParser_GetExpression_1.GetExpression)(context, i);
            if (newIndex === i)
                break;
            nodeListItems.push(nodeOtherItem);
            i = newIndex;
        } while (true);
    }
    parseNode = new FuncScriptParser_Main_1.ParseNode(FuncScriptParser_Main_1.ParseNodeType.List, index, i - tokenStart, nodeListItems);
    return { ParseNode: parseNode, NextIndex: i };
}
