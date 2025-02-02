import { ParseContext, ParseNode, ParseNodeType, ParseResult, SyntaxErrorData } from "../FuncScriptParser.Main";  
import { GetStringTemplate } from "./FuncScriptParser.GetStringTemplate";  
import { GetSimpleString } from "./FuncScriptParser.GetSimpleString";  
import { GetNumber } from "./FuncScriptParser.GetNumber";  
import { GetListExpression } from "./FuncScriptParser.GetListExpression";  
import { GetKvcExpression } from "./FuncScriptParser.GetKvcExpression";  
import { GetCaseExpression } from "./FuncScriptParser.GetCaseExpression";  
import { GetSwitchExpression } from "./FuncScriptParser.GetSwitchExpression";  
import { GetLambdaExpression } from "./FuncScriptParser.GetLambdaExpression";  
import { GetKeyWordLiteral } from "./FuncScriptParser.GetKeyWordLiteral";  
import { GetIdentifier } from "./FuncScriptParser.GetIdentifier";  
import { GetExpInParenthesis } from "./FuncScriptParser.GetExpInParenthesis";  
import { GetPrefixOperator } from "./FuncScriptParser.GetPrefixOperator";  
import { GetIfExpression } from "./FuncScriptParser.GetIfExpression";

export function GetUnit(context: ParseContext, index: number): ParseResult {  
    let parseNode: ParseNode | null = null;  
    let i: number;  

    const templateResult = GetStringTemplate(context, index);  
    i = templateResult.NextIndex;  
    if (i > index) {  
        return { ParseNode: templateResult.ParseNode, NextIndex: i };  
    }  

    const simpleStrResult = GetSimpleString(context, index);  
    i = simpleStrResult.NextIndex;  
    if (i > index) {  
        return { ParseNode: simpleStrResult.ParseNode, NextIndex: i };  
    }  

    const numberResult = GetNumber(context, index);  
    i = numberResult.NextIndex;  
    if (i > index) {  
        return { ParseNode: numberResult.ParseNode, NextIndex: i };  
    }  

    const listExprResult = GetListExpression(context, index);  
    i = listExprResult.NextIndex;  
    if (i > index) {  
        return { ParseNode: listExprResult.ParseNode, NextIndex: i };  
    }  

    const kvcExprResult = GetKvcExpression(context, index);  
    i = kvcExprResult.NextIndex;  
    if (i > index) {  
        return { ParseNode: kvcExprResult.ParseNode, NextIndex: i };  
    }  

    const caseExprResult = GetCaseExpression(context, index);  
    i = caseExprResult.NextIndex;  
    if (i > index) {  
        return { ParseNode: caseExprResult.ParseNode, NextIndex: i };  
    }  
    const ifExprResult =GetIfExpression(context, index);  
    i = ifExprResult.NextIndex;  
    if (i > index) {  
        return { ParseNode: ifExprResult.ParseNode, NextIndex: i };  
    }  


    const switchExprResult = GetSwitchExpression(context, index);  
    i = switchExprResult.NextIndex;  
    if (i > index) {  
        return { ParseNode: switchExprResult.ParseNode, NextIndex: i };  
    }  

    const lambdaExprResult = GetLambdaExpression(context, index);  
    i = lambdaExprResult.NextIndex;  
    if (i > index) {  
        return { ParseNode: lambdaExprResult.ParseNode, NextIndex: i };  
    }  

    const keywordLiteralResult = GetKeyWordLiteral(context, index);  
    i = keywordLiteralResult.NextIndex;  
    if (i > index) {  
        return { ParseNode: keywordLiteralResult.ParseNode, NextIndex: i };  
    }  

    const identResult = GetIdentifier(context, index, true);  
    i = identResult.NextIndex;  
    if (i > index) {  
        return { ParseNode: identResult.ParseNode, NextIndex: i };  
    }  

    const expInParenResult = GetExpInParenthesis(context, index);  
    i = expInParenResult.NextIndex;  
    if (i > index) {  
        return { ParseNode: expInParenResult.ParseNode, NextIndex: i };  
    }  

    const prefixOpResult = GetPrefixOperator(context, index);  
    i = prefixOpResult.NextIndex;  
    if (i > index) {  
        return { ParseNode: prefixOpResult.ParseNode, NextIndex: i };  
    }  

    return { ParseNode: parseNode, NextIndex: index };  

    
}  