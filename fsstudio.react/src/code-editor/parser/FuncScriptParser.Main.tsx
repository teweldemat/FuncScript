    export enum ParseNodeType {
        Comment,
        FunctionParameterList,
        FunctionCall,
        MemberAccess,
        Selection,
        InfixExpression,
        LiteralInteger,
        KeyWord,
        LiteralDouble,
        LiteralLong,
        Identifier,
        IdentifierList,
        Operator,
        LambdaExpression,
        ExpressionInBrace,
        LiteralString,
        StringTemplate,
        KeyValuePair,
        KeyValueCollection,
        List,
        Key,
        Case,
        GeneralInfixExpression,
        PrefixOperatorExpression,
        ReturnExpression
    }

    export class SyntaxErrorData {
        public Loc: number;
        public Length: number;
        public Message: string;

        constructor(loc: number, length: number, message: string) {
            this.Loc = loc;
            this.Message = message;
            this.Length = length;
        }
    }

    export class ParseNode {
        public NodeType: ParseNodeType;
        public Pos: number;
        public Length: number;
        public Children: ParseNode[];

        constructor(nodeType: ParseNodeType, pos: number, length: number, children: ParseNode[] = []) {
            this.NodeType = nodeType;
            this.Pos = pos;
            this.Length = length;
            this.Children = children;
        }
    }

    export const s_operatorSymols: string[][] = [
        ["^"],
        ["*", "/", "%"],
        ["+", "-"],
        [">=", "<=", "!=", ">", "<", "in"],
        ["=", "??", "?!", "?."],
        ["or", "and"],
        ["|"],
        [">>"]
    ];

    export const s_prefixOp: string[][] = [
        ["!", "not"], 
        ["-", "neg"]
    ];

    export const KW_RETURN = "return";
    export const KW_CASE = "case";
    export const KW_SWITCH = "switch";
    export const KW_ERROR = "fault";
    export const s_KeyWords: Set<string> = new Set<string>();

    (function initKeyWords() {
        s_KeyWords.add(KW_RETURN);
        s_KeyWords.add(KW_ERROR);
        s_KeyWords.add(KW_CASE);
        s_KeyWords.add(KW_SWITCH);
        s_KeyWords.add(KW_SWITCH);
    })();

    export interface ParseResult {
        ParseNode: ParseNode|null;
        NextIndex: number;
    }

    export interface KeyValueCollection {
    }

    export interface ParseContext {
        ReferenceProvider: KeyValueCollection;
        Expression: string;
        SyntaxErrors: SyntaxErrorData[];
    }

    
