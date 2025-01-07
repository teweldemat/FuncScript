using FuncScript.Block;
using FuncScript.Funcs.Math;
using System.Text;
using System.Text.RegularExpressions;
using FuncScript.Funcs.Logic;
using FuncScript.Model;
using FuncScript.Nodes;

namespace FuncScript.Core
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
            public String Message;

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

        static bool isCharWhiteSpace(char ch)
            => ch == ' ' ||
               ch == '\r' ||
               ch == '\t' ||
               ch == '\n';

        static int SkipSpace(String exp, int index)
        {
            int i = index;
            while (index < exp.Length)
            {
                if (isCharWhiteSpace(exp[index]))
                {
                    index++;
                }
                else
                {
                    i = GetCommentBlock(exp, index, out var nodeComment);
                    if (i == index)
                        break;
                    index = i;
                }
            }

            return index;
        }

        static int GetInt(String exp, bool allowNegative, int index, out string intVal, out ParseNode parseNode)
        {
            parseNode = null;
            int i = index;
            if (allowNegative)
                i = GetLiteralMatch(exp, i, "-");

            var i2 = i;
            while (i2 < exp.Length && char.IsDigit(exp[i2]))
                i2++;

            if (i == i2)
            {
                intVal = null;
                return index;
            }

            i = i2;

            intVal = exp.Substring(index, i - index);
            parseNode = new ParseNode(ParseNodeType.LiteralInteger, index, index - i);
            return i;
        }

        static int GetKeyWordLiteral(String exp, int index, out object literal, out ParseNode parseNode)
        {
            parseNode = null;
            var i = GetLiteralMatch(exp, index, "null");
            if (i > index)
            {
                literal = null;
            }
            else if ((i = GetLiteralMatch(exp, index, "true")) > index)
            {
                literal = true;
            }
            else if ((i = GetLiteralMatch(exp, index, "false")) > index)
            {
                literal = false;
            }
            else
            {
                literal = null;
                return index;
            }

            parseNode = new ParseNode(ParseNodeType.KeyWord, index, i - index);
            return i;
        }

        static int GetNumber(String exp, int index, out object number, out ParseNode parseNode,
            List<SyntaxErrorData> serros)
        {
            parseNode = null;
            var hasDecimal = false;
            var hasExp = false;
            var hasLong = false;
            number = null;
            int i = index;
            var i2 = GetInt(exp, true, i, out var intDigits, out var nodeDigits);
            if (i2 == i)
                return index;
            i = i2;

            i2 = GetLiteralMatch(exp, i, ".");
            if (i2 > i)
                hasDecimal = true;
            i = i2;
            if (hasDecimal)
            {
                i = GetInt(exp, false, i, out var decimalDigits, out var nodeDecimlaDigits);
            }

            i2 = GetLiteralMatch(exp, i, "E");
            if (i2 > i)
                hasExp = true;
            i = i2;
            String expDigits = null;
            ParseNode nodeExpDigits;
            if (hasExp)
                i = GetInt(exp, true, i, out expDigits, out nodeExpDigits);

            if (!hasDecimal) //if no decimal we check if there is the 'l' suffix
            {
                i2 = GetLiteralMatch(exp, i, "l");
                if (i2 > i)
                    hasLong = true;
                i = i2;
            }

            if (hasDecimal) //if it has decimal we treat it as 
            {
                if (!double.TryParse(exp.Substring(index, i - index), out var dval))
                {
                    serros.Add(new SyntaxErrorData(index, i - index,
                        $"{exp.Substring(index, i - index)} couldn't be parsed as floating point"));
                    return index; //we don't expect this to happen
                }

                number = dval;
                parseNode = new ParseNode(ParseNodeType.LiteralDouble, index, i - index);
                return i;
            }

            if (hasExp) //it e is included without decimal, zeros are appended to the digits
            {
                if (!int.TryParse(expDigits, out var e) || e < 0)
                {
                    serros.Add(new SyntaxErrorData(index, expDigits == null ? 0 : expDigits.Length,
                        $"Invalid exponentional {expDigits}"));
                    return index;
                }

                var maxLng = long.MaxValue.ToString();
                if (maxLng.Length + 1 < intDigits.Length + e) //check overflow by length
                {
                    serros.Add(new SyntaxErrorData(index, expDigits.Length,
                        $"Exponential {expDigits} is out of range"));
                    return index;
                }

                intDigits = intDigits + new string('0', e);
            }

            long longVal;

            if (hasLong) //if l suffix is found
            {
                if (!long.TryParse(intDigits, out longVal))
                {
                    serros.Add(new SyntaxErrorData(index, expDigits.Length,
                        $"{intDigits} couldn't be parsed to 64bit integer"));
                    return index;
                }

                number = longVal;
                parseNode = new ParseNode(ParseNodeType.LiteralLong, index, i - index);
                return i;
            }

            if (int.TryParse(intDigits, out var intVal)) //try parsing as int
            {
                number = intVal;
                parseNode = new ParseNode(ParseNodeType.LiteralInteger, index, i - index);
                return i;
            }

            if (long.TryParse(intDigits, out longVal)) //try parsing as long
            {
                number = longVal;
                parseNode = new ParseNode(ParseNodeType.LiteralLong, index, i - index);
                return i;
            }

            return index; //all failed
        }

        static bool IsIdentfierFirstChar(char ch)
        {
            return char.IsLetter(ch) || ch == '_';
        }

        static bool IsIdentfierOtherChar(char ch)
        {
            return char.IsLetterOrDigit(ch) || ch == '_';
        }

        static int GetSpaceLessString(String exp, int index, out String text, out ParseNode parseNode)
        {
            parseNode = null;
            text = null;
            if (index >= exp.Length)
                return index;
            var i = index;

            if (i >= exp.Length || isCharWhiteSpace(exp[i]))
                return index;
            i++;
            while (i < exp.Length && !isCharWhiteSpace(exp[i]))
                i++;

            text = exp.Substring(index, i - index);
            parseNode = new ParseNode(ParseNodeType.Identifier, index, i - index);
            return i;
        }

        static int GetIdentifier(String exp, int index, out String iden, out String idenLower, out ParseNode parseNode)
        {
            parseNode = null;
            iden = null;
            idenLower = null;
            if (index >= exp.Length)
                return index;
            var i = index;
            if (!IsIdentfierFirstChar(exp[i]))
                return index;
            i++;
            while (i < exp.Length && IsIdentfierOtherChar(exp[i]))
            {
                i++;
            }

            iden = exp.Substring(index, i - index);
            idenLower = iden.ToLower();
            if (s_KeyWords.Contains(idenLower))
                return index;
            parseNode = new ParseNode(ParseNodeType.Identifier, index, i - index);
            return i;
        }

        static int GetIdentifierList(String exp, int index, out List<String> idenList, out ParseNode parseNode)
        {
            parseNode = null;
            idenList = null;
            int i = SkipSpace(exp, index);
            //get open brace
            if (i >= exp.Length || exp[i++] != '(')
                return index;

            idenList = new List<string>();
            var parseNodes = new List<ParseNode>();
            //get first identifier
            i = SkipSpace(exp, i);
            int i2 = GetIdentifier(exp, i, out var iden, out var idenLower, out var nodeIden);
            if (i2 > i)
            {
                parseNodes.Add(nodeIden);
                idenList.Add(iden);
                i = i2;

                //get additional identifiers sperated by commas
                i = SkipSpace(exp, i);
                while (i < exp.Length)
                {
                    if (exp[i] != ',')
                        break;
                    i++;
                    i = SkipSpace(exp, i);
                    i2 = GetIdentifier(exp, i, out iden, out idenLower, out nodeIden);
                    if (i2 == i)
                        return index;
                    parseNodes.Add(nodeIden);
                    idenList.Add(iden);
                    i = i2;
                    i = SkipSpace(exp, i);
                }
            }

            //get close brace
            if (i >= exp.Length || exp[i++] != ')')
                return index;
            parseNode = new ParseNode(ParseNodeType.IdentiferList, index, i - index, parseNodes);
            return i;
        }

        static int GetOperator(IFsDataProvider parseContext, string[] candidates, string exp, int index,
            out string matechedOp, out IFsFunction oper,
            out ParseNode parseNode)
        {
            foreach (var op in candidates)
            {
                var i = GetLiteralMatch(exp, index, op);
                if (i <= index) continue;

                var func = parseContext.GetData(op);
//                if (func is not IFsFunction f) 
//                    continue;

                oper = func as IFsFunction;
                parseNode = new ParseNode(ParseNodeType.Operator, index, i - index);
                matechedOp = op;
                return i;
            }

            oper = null;
            parseNode = null;
            matechedOp = null;
            return index;
        }

        static int GetLambdaExpression(IFsDataProvider context, String exp, int index, out ExpressionFunction func,
            out ParseNode parseNode, List<SyntaxErrorData> serrors)
        {
            parseNode = null;
            func = null;

            var i = GetIdentifierList(exp, index, out var parms, out var nodesParams);
            if (i == index)
                return index;

            i = SkipSpace(exp, i);
            if (i >= exp.Length - 1) // we need two characters
                return index;
            var i2 = GetLiteralMatch(exp, i, "=>");
            if (i2 == i)
            {
                serrors.Add(new SyntaxErrorData(i, 0, "'=>' expected"));
                return index;
            }

            i += 2;
            i = SkipSpace(exp, i);
            var parmsSet = new HashSet<string>();
            foreach (var p in parms)
            {
                parmsSet.Add(p);
            }

            i2 = GetExpression(context, exp, i, out var defination, out var nodeDefination, serrors);
            if (i2 == i)
            {
                serrors.Add(new SyntaxErrorData(i, 0, "defination of lambda expression expected"));
                return index;
            }

            func = new ExpressionFunction(parms.ToArray(), defination);
            i = i2;
            parseNode = new ParseNode(ParseNodeType.LambdaExpression, index, i - index,
                new[] { nodesParams, nodeDefination });
            return i;
        }

        static int GetExpInParenthesis(IFsDataProvider infixFuncProvider, String exp, int index,
            out ExpressionBlock expression, out ParseNode parseNode, List<SyntaxErrorData> serrors)
        {
            parseNode = null;
            expression = null;
            var i = index;
            i = SkipSpace(exp, i);
            var i2 = GetLiteralMatch(exp, i, "(");
            if (i == i2)
                return index;
            i = i2;

            i = SkipSpace(exp, i);
            i2 = GetExpression(infixFuncProvider, exp, i, out expression, out var nodeExpression, serrors);
            if (i2 == i)
                expression = null;
            else
                i = i2;
            i = SkipSpace(exp, i);
            i2 = GetLiteralMatch(exp, i, ")");
            if (i == i2)
            {
                serrors.Add(new SyntaxErrorData(i, 0, "')' expected"));
                return index;
            }

            i = i2;
            if (expression == null)
                expression = new NullExpressionBlock();
            parseNode = new ParseNode(ParseNodeType.ExpressionInBrace, index, i - index, new[] { nodeExpression });
            return i;
        }
        
        //... (Other methods remain unchanged)
        
        // The rest of the code should also be modified accordingly.
    }
}
