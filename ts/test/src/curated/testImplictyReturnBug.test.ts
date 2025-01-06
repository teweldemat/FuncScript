import { Parse } from "funcscript.ts.core/src/parser/syntax/FuncScriptParser.Parse";
import { ParseContext, ParseNodeType } from "funcscript.ts.core/src/parser/FuncScriptParser.Main";

test("XColon12SemiXParsedCorrectly", () => {
  const expression = "x:12; x";
  const context: ParseContext = {
    Expression: expression,
    SyntaxErrors: []
  };

  const res = Parse(context);

  expect(res.ParseNode).not.toBeNull();
  expect(res.ParseNode!.NodeType).toBe(ParseNodeType.KeyValueCollection);
  expect(res.ParseNode!.Children.length).toBe(2);

  const kvpNode = res.ParseNode!.Children[0];
  expect(kvpNode.NodeType).toBe(ParseNodeType.KeyValuePair);

  const idNode = res.ParseNode!.Children[1];
  expect(idNode.NodeType).toBe(ParseNodeType.Identifier);
});