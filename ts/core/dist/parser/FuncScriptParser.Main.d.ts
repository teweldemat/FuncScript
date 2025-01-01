export declare enum ParseNodeType {
    Comment = 0,
    FunctionParameterList = 1,
    FunctionCall = 2,
    MemberAccess = 3,
    Selection = 4,
    InfixExpression = 5,
    LiteralInteger = 6,
    KeyWord = 7,
    LiteralDouble = 8,
    LiteralLong = 9,
    Identifier = 10,
    IdentifierList = 11,
    Operator = 12,
    LambdaExpression = 13,
    ExpressionInBrace = 14,
    LiteralString = 15,
    StringTemplate = 16,
    KeyValuePair = 17,
    KeyValueCollection = 18,
    List = 19,
    Key = 20,
    Case = 21,
    GeneralInfixExpression = 22,
    PrefixOperatorExpression = 23,
    ReturnExpression = 24
}
export declare class SyntaxErrorData {
    Loc: number;
    Length: number;
    Message: string;
    constructor(loc: number, length: number, message: string);
}
export declare class ParseNode {
    NodeType: ParseNodeType;
    Pos: number;
    Length: number;
    Children: ParseNode[];
    constructor(nodeType: ParseNodeType, pos: number, length: number, children?: ParseNode[]);
}
export declare const s_operatorSymols: string[][];
export declare const s_prefixOp: string[][];
export declare const KW_RETURN = "return";
export declare const KW_CASE = "case";
export declare const KW_SWITCH = "switch";
export declare const KW_ERROR = "fault";
export declare const s_KeyWords: Set<string>;
export interface ParseResult {
    ParseNode: ParseNode | null;
    NextIndex: number;
}
export interface ParseContext {
    Expression: string;
    SyntaxErrors: SyntaxErrorData[];
}
