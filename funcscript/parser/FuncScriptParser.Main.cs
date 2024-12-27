using funcscript.block;
using funcscript.funcs.math;
using funcscript.funcs.logic;

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
            IdentiferList,
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
            DataConnection,
            NormalErrorSink,
            SigSequence,
            ErrorKeyWord,
            SignalConnection,
            GeneralInfixExpression,
            PrefixOperatorExpression
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
            public IList<ParseNode> Childs;

            public ParseNode(ParseNodeType type, int pos, int length)
                : this(type, pos, length, Array.Empty<ParseNode>())
            {
            }

            public ParseNode(ParseNodeType nodeType, int pos, int length, IList<ParseNode> childs)
            {
                NodeType = nodeType;
                Pos = pos;
                Length = length;
                Childs = childs;
            }
        }

        static string[][] s_operatorSymols =
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

        record ParseResult(ExpressionBlock Expresion, ParseNode Node, int NextIndex);

        // static int GetPrefixOperator(IFsDataProvider parseContext, string exp, int index, out ExpressionBlock prog,
        //     out ParseNode parseNode, List<SyntaxErrorData> serrors)
        // {
        //     int i = 0;
        //     string oper = null;
        //     foreach (var op in s_prefixOp)
        //     {
        //         i = GetLiteralMatch(exp, index, op[0]);
        //         if (i > index)
        //         {
        //             oper = op[1];
        //             break;
        //         }
        //     }
        //
        //     if (i == index)
        //     {
        //         prog = null;
        //         parseNode = null;
        //         return index;
        //     }
        //
        //     i = SkipSpace(exp, i);
        //     var func = parseContext.Get(oper);
        //     if (func == null)
        //     {
        //         serrors.Add(new SyntaxErrorData(index, i - index, $"Prefix operator {oper} not defined"));
        //         prog = null;
        //         parseNode = null;
        //         return index;
        //     }
        //
        //     var i2 = GetCallAndMemberAccess(parseContext, exp, i, out var operand, out var operandNode, serrors);
        //     if (i2 == i)
        //     {
        //         serrors.Add(new SyntaxErrorData(i, 0, $"Operant for {oper} expected"));
        //         prog = null;
        //         parseNode = null;
        //         return index;
        //     }
        //
        //     i = SkipSpace(exp, i2);
        //
        //     prog = new FunctionCallExpression
        //     {
        //         Provider = parseContext,
        //         Function = new LiteralBlock(func),
        //         Parameters = new[] { operand },
        //         CodePos = index,
        //         CodeLength = i - index,
        //     };
        //     parseNode = new ParseNode(ParseNodeType.PrefixOperatorExpression, index, i - index);
        //     return i;
        // }
    }
}