import { ParseContext, ParseNodeType } from "funcscript.ts.core/src/parser/FuncScriptParser.Main";
import { Parse } from "funcscript.ts.core/src/parser/syntax/FuncScriptParser.Parse";

test("NakedKeyValueCollectionWithImplicitReturn parsed correctly", () => {
  const expression = "x:12;x*2;";
  const c: ParseContext = {
    Expression: expression,
    SyntaxErrors: []
  };

  const res = Parse(c);

  expect(res.ParseNode).toBeDefined();
  expect(res.ParseNode!.NodeType).toBe(ParseNodeType.KeyValueCollection);
  expect(res.ParseNode!.Children.length).toBe(2);

  const keyValuePairNode = res.ParseNode!.Children[0];
  expect(keyValuePairNode.NodeType).toBe(ParseNodeType.KeyValuePair);

  const infixExpressionNode = res.ParseNode!.Children[1];
  expect(infixExpressionNode.NodeType).toBe(ParseNodeType.InfixExpression);
});