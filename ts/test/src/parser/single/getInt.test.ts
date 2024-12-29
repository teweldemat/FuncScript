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