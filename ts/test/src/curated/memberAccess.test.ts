import { Parse } from "funcscript.ts.core/src/parser/syntax/FuncScriptParser.Parse";
import { ParseContext, ParseNodeType } from "funcscript.ts.core/src/parser/FuncScriptParser.Main";

test("MemberAccessParsedCorrectly", () => {
  const expression = "x.y";
  const c: ParseContext = {
    Expression: expression,
    SyntaxErrors: []
  };

  const res = Parse(c);

  expect(res.ParseNode).toBeDefined();
  expect(res.ParseNode!.NodeType).toBe(ParseNodeType.MemberAccess);

  expect(res.ParseNode!.Children.length).toBe(2);
  const leftChild = res.ParseNode!.Children[0];
  const rightChild = res.ParseNode!.Children[1];

  expect(leftChild.NodeType).toBe(ParseNodeType.Identifier);
  expect(leftChild.Pos).toBe(0);
  expect(leftChild.Length).toBe(1); 

  expect(rightChild.NodeType).toBe(ParseNodeType.Identifier);
  expect(rightChild.Pos).toBe(2);
  expect(rightChild.Length).toBe(1);
});