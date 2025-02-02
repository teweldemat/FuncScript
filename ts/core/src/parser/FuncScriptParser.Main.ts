    export enum ParseNodeType {
        Comment,//0
        FunctionParameterList,//1
        FunctionCall,//2
        MemberAccess,//3
        Selection,//4
        InfixExpression,//5
        LiteralInteger,//6
        KeyWord,//7
        LiteralDouble,//8
        LiteralLong,//9
        Identifier,//10
        IdentifierList,//11
        Operator,//12
        LambdaExpression,//13
        ExpressionInBrace,//14
        LiteralString,//15
        StringTemplate,//16
        KeyValuePair,//17
        KeyValueCollection,//19
        List,//19
        Key,//20
        Case,//21
        PrefixOperatorExpression,//22
        ReturnExpression,//23
        IfExpression//24
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

    export const s_operatorSymbols: string[][] = [
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
    export const KW_IF = "if";
    export const KW_THEN = "then";
    export const KW_ELSE = "else";

    export const s_KeyWords: Set<string> = new Set<string>();

    (function initKeyWords() {
        s_KeyWords.add(KW_RETURN);
        s_KeyWords.add(KW_ERROR);
        s_KeyWords.add(KW_CASE);
        s_KeyWords.add(KW_SWITCH);
        s_KeyWords.add(KW_IF);
        s_KeyWords.add(KW_THEN);
        s_KeyWords.add(KW_ELSE);
    })();

    export interface ParseResult {
        ParseNode: ParseNode|null;
        NextIndex: number;
    }


    export interface ParseContext {
        Expression: string;
        SyntaxErrors: SyntaxErrorData[];
    }

    
    
