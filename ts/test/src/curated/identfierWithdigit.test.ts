import { ParseContext, ParseNodeType } from "funcscript.ts.core/src/parser/FuncScriptParser.Main";
import { Parse } from "funcscript.ts.core/src/parser/syntax/FuncScriptParser.Parse";

test("Identifier with digit parse test", () => {
  const c: ParseContext = {
    Expression: "x2",
    SyntaxErrors: []
  };

  const res = Parse(c);

  expect(res.ParseNode).toBeDefined();
  expect(res.ParseNode!.NodeType).toBe(ParseNodeType.Identifier);

  expect(res.ParseNode!.Pos).toBe(0);
  expect(res.ParseNode!.Length).toBe(2);

  expect(res.ParseNode!.Children.length).toBe(0);
});