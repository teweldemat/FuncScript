import { ParseContext, ParseNodeType } from "funcscript.ts.core/src/parser/FuncScriptParser.Main";
import { Parse } from "funcscript.ts.core/src/parser/syntax/FuncScriptParser.Parse";

test("KeyValueWithReturnParseCorrectly", () => {
    const kv = "x:5";
    const ret = "return x";
    const expression = `{${kv};${ret};}`;
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
    expect(keyValuePairNode.Pos).toBe(1);
    expect(keyValuePairNode.Length).toBe(kv.length);
    expect(keyValuePairNode.Children.length).toBe(2);

    const keyNode = keyValuePairNode.Children[0];
    expect(keyNode.NodeType).toBe(ParseNodeType.Key);
    expect(keyNode.Pos).toBe(1);
    expect(keyNode.Length).toBe(1);

    const valueNode = keyValuePairNode.Children[1];
    expect(valueNode.NodeType).toBe(ParseNodeType.LiteralInteger);
    expect(valueNode.Pos).toBe(1 + kv.indexOf(':') + 1);
    expect(valueNode.Length).toBe(1);

    const returnNode = res.ParseNode!.Children[1];
    expect(returnNode.NodeType).toBe(ParseNodeType.ReturnExpression);
    expect(returnNode.Pos).toBe(1 + kv.length + 1);
    expect(returnNode.Length).toBe(ret.length);
    expect(returnNode.Children.length).toBe(2);

    const keyWordNode = returnNode.Children[0];
    expect(keyWordNode.NodeType).toBe(ParseNodeType.KeyWord);
    expect(keyWordNode.Pos).toBe(1 + kv.length + 1);
    expect(keyWordNode.Length).toBe("return".length);

    const identifierNode = returnNode.Children[1];
    expect(identifierNode.NodeType).toBe(ParseNodeType.Identifier);
    expect(identifierNode.Pos).toBe(
        1 + kv.length + 1 + "return".length + 1
    );
    expect(identifierNode.Length).toBe(1);
});