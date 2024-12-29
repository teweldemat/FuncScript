import { Parse } from "funcscript.ts.core/src/parser/syntax/FuncScriptParser.Parse";
import { ParseContext, ParseNodeType } from "funcscript.ts.core/src/parser/FuncScriptParser.Main";

test("Integer token parsed correctly", () => {
  const c: ParseContext = {
    Expression: "5",
    SyntaxErrors: []
  };

  const res = Parse(c);
  expect(res.ParseNode).toBeDefined();
  expect(res.ParseNode!.NodeType).toBe(ParseNodeType.LiteralInteger);
  expect(res.ParseNode!.Children?.length ?? 0).toBe(0);
  expect(res.ParseNode!.Pos).toBe(0);
  expect(res.ParseNode!.Length).toBe(1);
});

test("Simple infix tokens parsed correctly", () => {
    const c: ParseContext = {
      Expression: "5+6",
      SyntaxErrors: []
    };
  
    const res = Parse(c);
    
    
    expect(res.ParseNode).toBeDefined();
    
    expect(res.ParseNode!.NodeType).toBe(ParseNodeType.InfixExpression);
    expect(res.ParseNode!.Children?.length ?? 0).toBe(3);
    expect(res.ParseNode!.Pos).toBe(0);
    expect(res.ParseNode!.Length).toBe(3);

    const oper1=res.ParseNode?.Children[0]
    const op=res.ParseNode?.Children[1]
    const oper2=res.ParseNode?.Children[2]
    
    
    expect(oper1!.NodeType).toBe(ParseNodeType.LiteralInteger);
    expect(oper1!.Children?.length ?? 0).toBe(0);
    expect(oper1!.Pos).toBe(0);
    expect(oper1!.Length).toBe(1);
  
    expect(op!.NodeType).toBe(ParseNodeType.Operator);
    expect(op!.Children?.length ?? 0).toBe(0);
    expect(op!.Pos).toBe(1);
    expect(op!.Length).toBe(1);


    expect(oper2!.NodeType).toBe(ParseNodeType.LiteralInteger);
    expect(oper2!.Children?.length ?? 0).toBe(0);
    expect(oper2!.Pos).toBe(2);
    expect(oper2!.Length).toBe(1);

  });