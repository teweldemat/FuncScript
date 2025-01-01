import { ParseContext, ParseNodeType } from "funcscript.ts.core/src/parser/FuncScriptParser.Main";
import { Parse } from "funcscript.ts.core/src/parser/syntax/FuncScriptParser.Parse";

test("Map lambda expression parsed correctly", () => {
    const c: ParseContext = {
      Expression: "x map (a)=>b",
      SyntaxErrors: []
    };
  
    const res = Parse(c);
  
    expect(res.ParseNode).toBeDefined();
    expect(res.ParseNode!.NodeType).toBe(ParseNodeType.InfixExpression);
  
    expect(res.ParseNode!.Children.length).toBe(3);
  
    const xNode = res.ParseNode!.Children[0];
    expect(xNode.NodeType).toBe(ParseNodeType.Identifier);
    expect(xNode.Pos).toBe(0);
    expect(xNode.Length).toBe(1);
  
    const mapNode = res.ParseNode!.Children[1];
    expect(mapNode.NodeType).toBe(ParseNodeType.Identifier);
    expect(mapNode.Pos).toBe(2);
    expect(mapNode.Length).toBe(3);
  
    const lambdaNode = res.ParseNode!.Children[2];
    expect(lambdaNode.NodeType).toBe(ParseNodeType.LambdaExpression);
    expect(lambdaNode.Pos).toBe(6);
    expect(lambdaNode.Length).toBe(6);
  
    expect(lambdaNode.Children.length).toBe(2);
  
    const paramListNode = lambdaNode.Children[0];
    expect(paramListNode.NodeType).toBe(ParseNodeType.IdentifierList);
    expect(paramListNode.Pos).toBe(6);
    expect(paramListNode.Length).toBe(3);
  
    expect(paramListNode.Children.length).toBe(1);
    const aNode = paramListNode.Children[0];
    expect(aNode.NodeType).toBe(ParseNodeType.Identifier);
    expect(aNode.Pos).toBe(7);
    expect(aNode.Length).toBe(1);
  
    const bodyNode = lambdaNode.Children[1];
    expect(bodyNode.NodeType).toBe(ParseNodeType.Identifier);
    expect(bodyNode.Pos).toBe(11);
    expect(bodyNode.Length).toBe(1);
  });