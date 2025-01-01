import { ParseContext, ParseNode, ParseNodeType, ParseResult } from "../FuncScriptParser.Main";
import { GetFunctionCallParametersList } from "./FuncScriptParser.GetFunctionCallParametersList";
import { GetKvcExpression } from "./FuncScriptParser.GetKvcExpression";
import { GetMemberAccess } from "./FuncScriptParser.GetMemberAccess";
import { GetUnit } from "./FuncScriptParser.GetUnit";
import { SkipSpace } from "./FuncScriptParser.SkipSpace";

export function GetCallAndMemberAccess(context: ParseContext, index: number): ParseResult {
    let parseNode: ParseNode | null = null;
    const i1 = SkipSpace(context, index).NextIndex;
    let {  ParseNode: tempParseNode,NextIndex: i } = GetUnit(context, i1);
    
    if (i === index) {
        return { ParseNode: null, NextIndex: index };
    }
    
    parseNode = tempParseNode;

    while (true) {
        let {ParseNode: nodeParList, NextIndex: i2 } = GetFunctionCallParametersList(context,  i);
        if (i2 > i) {
            i = i2;
            parseNode = new ParseNode(ParseNodeType.FunctionCall, index, i - index, [parseNode!, nodeParList!]);
            continue;
        }

        let { ParseNode:nodeMemberAccess, NextIndex:i2Member } = GetMemberAccess(context,  i);
        if (i2Member > i) {
            i = i2Member;
            parseNode = new ParseNode(ParseNodeType.MemberAccess, index, i - index, [parseNode!, nodeMemberAccess!]);
            continue;
        }

        let { ParseNode: nodeKvc, NextIndex: i2Kvc } = GetKvcExpression(context,  i);
        if (i2Kvc > i) {
            i = i2Kvc;
            parseNode = new ParseNode(ParseNodeType.Selection, index, i - index, [parseNode!, nodeKvc!]);
            continue;
        }

        return { ParseNode: parseNode, NextIndex: i };
    }
}