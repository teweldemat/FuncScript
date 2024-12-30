import { ParseContext, ParseNodeType } from "funcscript.ts.core/src/parser/FuncScriptParser.Main";
import { Parse } from "funcscript.ts.core/src/parser/syntax/FuncScriptParser.Parse";

test("Two addition operators are parsed correctly", () => {
    const expression = "4+5+8";
    const c: ParseContext = {
      Expression: expression,
      SyntaxErrors: []
    };

    const result = Parse(c);

    expect(result.ParseNode).not.toBeNull();
    expect(result.ParseNode!.NodeType).toBe(ParseNodeType.InfixExpression);
    expect(result.ParseNode!.Children.length).toBe(5);

    const firstNum = result.ParseNode!.Children[0];
    expect(firstNum.NodeType).toBe(ParseNodeType.LiteralInteger);
    expect(firstNum.Pos).toBe(0);
    expect(firstNum.Length).toBe(1);

    const firstOp = result.ParseNode!.Children[1];
    expect(firstOp.NodeType).toBe(ParseNodeType.Operator);
    expect(firstOp.Pos).toBe(1);
    expect(firstOp.Length).toBe(1);

    const secondNum = result.ParseNode!.Children[2];
    expect(secondNum.NodeType).toBe(ParseNodeType.LiteralInteger);
    expect(secondNum.Pos).toBe(2);
    expect(secondNum.Length).toBe(1);

    const secondOp = result.ParseNode!.Children[3];
    expect(secondOp.NodeType).toBe(ParseNodeType.Operator);
    expect(secondOp.Pos).toBe(3);
    expect(secondOp.Length).toBe(1);

    const thirdNum = result.ParseNode!.Children[4];
    expect(thirdNum.NodeType).toBe(ParseNodeType.LiteralInteger);
    expect(thirdNum.Pos).toBe(4);
    expect(thirdNum.Length).toBe(1);
});