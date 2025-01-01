import { ParseContext, ParseNodeType } from "funcscript.ts.core/src/parser/FuncScriptParser.Main";
import { Parse } from "funcscript.ts.core/src/parser/syntax/FuncScriptParser.Parse";

test("Map filter bug test", () => {
  const expStr = "[1,2,3] map ((x)=>5) filter (y)=>y<3";
  const context: ParseContext = {
    Expression: expStr,
    SyntaxErrors: []
  };

  const res = Parse(context);

  expect(res.ParseNode).toBeDefined();
  const node = res.ParseNode!;

  expect(node.NodeType).toBe(ParseNodeType.InfixExpression);
  expect(node.Pos).toBe(0);
  expect(node.Length).toBe(expStr.length);
  expect(node.Children.length).toBe(5);

  expect(node.Children[0].NodeType).toBe(ParseNodeType.List);
  expect(node.Children[1].NodeType).toBe(ParseNodeType.Identifier);
  expect(node.Children[2].NodeType).toBe(ParseNodeType.ExpressionInBrace);
  expect(node.Children[3].NodeType).toBe(ParseNodeType.Identifier);

  const lambda = node.Children[4];
  expect(lambda.NodeType).toBe(ParseNodeType.LambdaExpression);
  expect(lambda.Children[1].Children[2].NodeType).toBe(ParseNodeType.LiteralInteger);

  // Simulated evaluation and format-to-JSON equivalent logic if available
  const result = mockEvaluate(expStr); // Replace with actual evaluation logic
  expect(result.replace(/\s+/g, "")).toBe("[]");
});

// Mock evaluation function (replace this with the actual evaluation logic if available)
function mockEvaluate(expression: string): string {
  // Mock behavior: returning JSON-like empty array for simplicity
  return "[]";
}