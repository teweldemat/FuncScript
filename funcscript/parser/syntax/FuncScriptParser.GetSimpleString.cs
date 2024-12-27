using System.Text;
using funcscript.model;

namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        static int GetSimpleString(KeyValueCollection provider, string exp, int index, out String str, out ParseNode parseNode,
            List<SyntaxErrorData> syntaxErrors)
        {
            var i = GetSimpleString(provider, exp, "\"", index, out str, out parseNode, syntaxErrors);
            if (i > index)
                return i;
            return GetSimpleString(provider, exp, "'", index, out str, out parseNode, syntaxErrors);
        }

        static int GetSimpleString(KeyValueCollection provider, string exp, string delimator, int index, out String str, out ParseNode parseNode,
            List<SyntaxErrorData> syntaxErrors)
        {
            parseNode = null;
            str = null;
            var i = GetLiteralMatch(exp, index, delimator);
            if (i == index)
                return index;
            int i2;
            var sb = new StringBuilder();
            while (true)
            {
                i2 = GetLiteralMatch(exp, i, @"\n");
                if (i2 > i)
                {
                    i = i2;
                    sb.Append('\n');
                    continue;
                }

                i2 = GetLiteralMatch(exp, i, @"\t");
                if (i2 > i)
                {
                    i = i2;
                    sb.Append('\t');
                    continue;
                }

                i2 = GetLiteralMatch(exp, i, @"\\");
                if (i2 > i)
                {
                    i = i2;
                    sb.Append('\\');
                    continue;
                }

                i2 = GetLiteralMatch(exp, i, @"\u");
                if (i2 > i)
                {
                    if (i + 6 <= exp.Length) // Checking if there is enough room for 4 hex digits
                    {
                        var unicodeStr = exp.Substring(i + 2, 4);
                        if (int.TryParse(unicodeStr, System.Globalization.NumberStyles.HexNumber, null,
                                out int charValue))
                        {
                            sb.Append((char)charValue);
                            i += 6; // Move past the "\uXXXX"
                            continue;
                        }
                    }
                }

                i2 = GetLiteralMatch(exp, i, $@"\{delimator}");
                if (i2 > i)
                {
                    sb.Append(delimator);
                    i = i2;
                    continue;
                }

                if (i >= exp.Length || GetLiteralMatch(exp, i, delimator) > i)
                    break;
                sb.Append(exp[i]);
                i++;
            }

            i2 = GetLiteralMatch(exp, i, delimator);
            if (i2 == i)
            {
                syntaxErrors.Add(new SyntaxErrorData(i, 0, $"'{delimator}' expected"));
                return index;
            }

            i = i2;
            str = sb.ToString();
            parseNode = new ParseNode(ParseNodeType.LiteralString, index, i - index);
            return i;
        }
    }
}