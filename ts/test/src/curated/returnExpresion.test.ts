import { ParseContext, ParseNodeType } from "funcscript.ts.core/src/parser/FuncScriptParser.Main";
import { Parse } from "funcscript.ts.core/src/parser/syntax/FuncScriptParser.Parse";

test("Return expression parsed correctly", () => {
    const c: ParseContext = {
        Expression: "{return 2}",
        SyntaxErrors: []
    };

    const res = Parse(c);

    expect(res.ParseNode).toBeDefined();
    expect(res.ParseNode!.NodeType).toBe(ParseNodeType.KeyValueCollection);

    expect(res.ParseNode!.Children.length).toBe(1);

    const returnNode = res.ParseNode!.Children[0];
    expect(returnNode.NodeType).toBe(ParseNodeType.ReturnExpression);
    expect(returnNode.Pos).toBe(1);
    expect(returnNode.Length).toBe(8);

    expect(returnNode.Children.length).toBe(2);

    const keyWordNode = returnNode.Children[0];
    expect(keyWordNode.NodeType).toBe(ParseNodeType.KeyWord);
    expect(keyWordNode.Pos).toBe(1);
    expect(keyWordNode.Length).toBe(6);

    const literalNode = returnNode.Children[1];
    expect(literalNode.NodeType).toBe(ParseNodeType.LiteralInteger);
    expect(literalNode.Pos).toBe(8);
    expect(literalNode.Length).toBe(1);
});