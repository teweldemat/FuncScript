import { ParseContext } from "funcscript.ts.core/src/parser/FuncScriptParser.Main";
import { GetIdentifier } from "funcscript.ts.core/src/parser/syntax/FuncScriptParser.GetIdentifier";

describe("GetIdentifier", () => {
  test("Returns all null when index is out of range", () => {
    const context: ParseContext = { Expression: "", SyntaxErrors: [] } as ParseContext;
    const result = GetIdentifier(context, 0, true);
    expect(result.Iden).toBeNull();
    expect(result.IdenLower).toBeNull();
    expect(result.ParentRef).toBe(false);
    expect(result.ParseNode).toBeNull();
    expect(result.NextIndex).toBe(0);
  });

  test("Handles parent reference when '^' is present", () => {
    const context: ParseContext = { Expression: "^parentId", SyntaxErrors: [] } as ParseContext;
    const result = GetIdentifier(context, 0, true);
    expect(result.Iden).toBe("parentId");
    expect(result.IdenLower).toBe("parentid");
    expect(result.ParentRef).toBe(true);
    expect(result.ParseNode).not.toBeNull();
    expect(result.ParseNode?.Pos).toBe(0);
    expect(result.ParseNode?.Length).toBe(1 + "parentId".length);
    expect(result.NextIndex).toBe(9);
  });

  test("Returns valid identifier when no '^' and index is valid", () => {
    const context: ParseContext = { Expression: "myVariable", SyntaxErrors: [] } as ParseContext;
    const result = GetIdentifier(context, 0, true);
    expect(result.Iden).toBe("myVariable");
    expect(result.IdenLower).toBe("myvariable");
    expect(result.ParentRef).toBe(false);
    expect(result.ParseNode).not.toBeNull();
    expect(result.ParseNode?.Pos).toBe(0);
    expect(result.ParseNode?.Length).toBe("myVariable".length);
    expect(result.NextIndex).toBe(10);
  });

  test("Returns null if first char is not valid identifier start", () => {
    const context: ParseContext = { Expression: "1someId", SyntaxErrors: [] } as ParseContext;
    const result = GetIdentifier(context, 0, true);
    expect(result.Iden).toBeNull();
    expect(result.IdenLower).toBeNull();
    expect(result.ParentRef).toBe(false);
    expect(result.ParseNode).toBeNull();
    expect(result.NextIndex).toBe(0);
  });

  test("Stops parsing identifier when encountering invalid char", () => {
    const context: ParseContext = { Expression: "someId!", SyntaxErrors: [] } as ParseContext;
    const result = GetIdentifier(context, 0, false);
    expect(result.Iden).toBe("someId");
    expect(result.IdenLower).toBe("someid");
    expect(result.ParentRef).toBe(false);
    expect(result.ParseNode).not.toBeNull();
    expect(result.ParseNode?.Pos).toBe(0);
    expect(result.ParseNode?.Length).toBe("someId".length);
    expect(result.NextIndex).toBe(6);
  });

  test("Parses x123 as a valid identifier", () => {
    const context: ParseContext = { Expression: "x123", SyntaxErrors: [] } as ParseContext;
    const result = GetIdentifier(context, 0, true);
    expect(result.Iden).toBe("x123");
    expect(result.IdenLower).toBe("x123");
    expect(result.ParentRef).toBe(false);
    expect(result.ParseNode).not.toBeNull();
    expect(result.ParseNode?.Pos).toBe(0);
    expect(result.ParseNode?.Length).toBe(4);
    expect(result.NextIndex).toBe(4);
  });
});