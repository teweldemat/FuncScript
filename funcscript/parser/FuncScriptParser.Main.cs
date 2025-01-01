using funcscript.block;
using funcscript.funcs.math;
using funcscript.funcs.logic;
using funcscript.model;

namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        public enum ParseNodeType
        {
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
            PrefixOperatorExpression,
            ReturnExpression
        }

        public class SyntaxErrorData
        {
            public int Loc;
            public int Length;
            public string Message;

            public SyntaxErrorData(int loc, int length, string message)
            {
                Loc = loc;
                Message = message;
                Length = length;
            }
        }

        public class ParseNode
        {
            public ParseNodeType NodeType;
            public int Pos;
            public int Length;
            public IList<ParseNode> Children;

            public ParseNode(ParseNodeType type, int pos, int length)
                : this(type, pos, length, Array.Empty<ParseNode>())
            {
            }

            public ParseNode(ParseNodeType nodeType, int pos, int length, IList<ParseNode> children)
            {
                NodeType = nodeType;
                Pos = pos;
                Length = length;
                Children = children;
            }
        }

        static string[][] s_operatorSymbols =
        {
            new[] { "^" },
            new[] { "*", "/", "%" },
            new[] { "+", "-" },
            new[] { ">=", "<=", "!=", ">", "<", "in" },
            new[] { "=", "??", "?!", "?." },
            new[] { "or", "and" },
            new[] { "|" },
            new[] { ">>" },
        };

        private static string[][] s_prefixOp =
            { new string[] { "!", NotFunction.SYMBOL }, new string[] { "-", NegateFunction.SYMBOL } };

        const string KW_RETURN = "return";
        const string KW_CASE = "case";
        const string KW_SWITCH = "switch";
        private const string KW_ERROR = "fault";
        static HashSet<string> s_KeyWords;

        static FuncScriptParser()
        {
            s_KeyWords = new HashSet<string>();
            s_KeyWords.Add(KW_RETURN);
            s_KeyWords.Add(KW_ERROR);
            s_KeyWords.Add(KW_CASE);
            s_KeyWords.Add(KW_SWITCH);
            s_KeyWords.Add(KW_SWITCH);
        }

        public record ParseResult(ParseNode ParseNode, int NextIndex);
        public record ExpressionBlockResult(ExpressionBlock Block, ParseNode ParseNode, int NextIndex):ParseResult(ParseNode,NextIndex);
        public record ParseContext(KeyValueCollection ReferenceProvider, string Expression, List<SyntaxErrorData> SyntaxErrors);
    }
}