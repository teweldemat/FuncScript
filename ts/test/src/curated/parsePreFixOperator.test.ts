import { ParseContext, ParseNodeType } from "funcscript.ts.core/src/parser/FuncScriptParser.Main";
import { Parse } from "funcscript.ts.core/src/parser/syntax/FuncScriptParser.Parse";

test("Parse not true correctly", () => {
  const c: ParseContext = {
    Expression: "!true",
    SyntaxErrors: []
  };

  const res = Parse(c);

  expect(res.ParseNode).not.toBeNull();
  expect(res.ParseNode!.NodeType).toBe(ParseNodeType.PrefixOperatorExpression);

  expect(res.ParseNode!.Pos).toBe(0);
  expect(res.ParseNode!.Length).toBe(5);
  expect(res.ParseNode!.Children.length).toBe(2);

  const operatorNode = res.ParseNode!.Children[0];
  expect(operatorNode.NodeType).toBe(ParseNodeType.Operator);
  expect(operatorNode.Pos).toBe(0);
  expect(operatorNode.Length).toBe(1);

  const operandNode = res.ParseNode!.Children[1];
  expect(operandNode.NodeType).toBe(ParseNodeType.KeyWord);
  expect(operandNode.Pos).toBe(1);
  expect(operandNode.Length).toBe(4);
});