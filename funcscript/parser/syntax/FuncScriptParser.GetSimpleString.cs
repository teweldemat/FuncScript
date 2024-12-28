using System.Text;
using funcscript.model;

namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        record GetStringResult(string Str, ParseNode Node,int NextIndex);
        static GetStringResult GetSimpleString(ParseContext context, int index)
        {
            var res = GetSimpleString(context,  "\"", index);
            if (res.NextIndex > index)
                return res;
            return GetSimpleString(context,  "'", index);
        }

        static GetStringResult GetSimpleString(ParseContext context,  string delimator, int index)
        {
            ParseNode parseNode = null;
            String str = null;
            var i = GetLiteralMatch(context, index, delimator).NextIndex;
            if (i == index)
                return new GetStringResult(null,null,index);
            int i2;
            var sb = new StringBuilder();
            while (true)
            {
                i2 = GetLiteralMatch(context, i, @"\n").NextIndex;
                if (i2 > i)
                {
                    i = i2;
                    sb.Append('\n');
                    continue;
                }

                i2 = GetLiteralMatch(context, i, @"\t").NextIndex;
                if (i2 > i)
                {
                    i = i2;
                    sb.Append('\t');
                    continue;
                }

                i2 = GetLiteralMatch(context, i, @"\\").NextIndex;
                if (i2 > i)
                {
                    i = i2;
                    sb.Append('\\');
                    continue;
                }

                i2 = GetLiteralMatch(context, i, @"\u").NextIndex;
                if (i2 > i)
                {
                    if (i + 6 <= context.Expression.Length) // Checking if there is enough room for 4 hex digits
                    {
                        var unicodeStr = context.Expression.Substring(i + 2, 4);
                        if (int.TryParse(unicodeStr, System.Globalization.NumberStyles.HexNumber, null,
                                out int charValue))
                        {
                            sb.Append((char)charValue);
                            i += 6; // Move past the "\uXXXX"
                            continue;
                        }
                    }
                }

                i2 = GetLiteralMatch(context, i, $@"\{delimator}").NextIndex;
                if (i2 > i)
                {
                    sb.Append(delimator);
                    i = i2;
                    continue;
                }

                if (i >= context.Expression.Length || GetLiteralMatch(context, i, delimator).NextIndex > i)
                    break;
                sb.Append(context.Expression[i]);
                i++;
            }

            i2 = GetLiteralMatch(context, i, delimator).NextIndex;
            if (i2 == i)
            {
                context.Serrors.Add(new SyntaxErrorData(i, 0, $"'{delimator}' expected"));
                return new GetStringResult(null,null,index);
            }

            i = i2;
            str = sb.ToString();
            parseNode = new ParseNode(ParseNodeType.LiteralString, index, i - index);
            return new GetStringResult(str,parseNode,i);
        }
    }
}