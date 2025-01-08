using FuncScript.Block;
using FuncScript.Funcs.Math;
using FuncScript.Model;

namespace FuncScript.Core
{
    public partial class FuncScriptParser
    {
        record GetNumberResult(object Number, ParseNode ParseNode, int NextIndex)
            : ParseResult(ParseNode, NextIndex);

        static GetNumberResult GetNumber(ParseContext context, int index)
        {
            ParseNode parseNode = null;
            object number = null;
            bool hasDecimal = false;
            bool hasExp = false;
            bool hasLong = false;

            int i = index;

            // 1. Parse initial integer part (sign allowed)
            var (intDigits, nodeDigits, i2) = GetInt(context, true, i);
            if (i2 == i)
                return new GetNumberResult(null, null, index);

            i = i2;

            // 2. Check if we have a decimal point. If next char after '.' is
            //    a letter other than 'e' or 'E', do NOT treat it as decimal.
            var dotMatch = GetLiteralMatch(context, i, ".");
            if (dotMatch.NextIndex > i)
            {
                // Look ahead
                bool parseAsDecimal = false;
                if (dotMatch.NextIndex < context.Expression.Length)
                {
                    char nextChar = context.Expression[dotMatch.NextIndex];
                    // If next char is end-of-string boundary, digit, 'e', 'E',
                    // or some non-letter operator, we parse decimal.
                    // We also allow "12." -> parse as double.
                    // But "5.a" -> do NOT parse decimal (consume only '5').
                    if (!char.IsLetter(nextChar) || nextChar == 'e' || nextChar == 'E')
                    {
                        parseAsDecimal = true;
                    }
                }
                else
                {
                    // '.' is the last character -> parse as decimal, e.g. "12."
                    parseAsDecimal = true;
                }

                if (parseAsDecimal)
                {
                    hasDecimal = true;
                    i = dotMatch.NextIndex;

                    // Parse optional digits after the decimal point
                    var (decimalDigits, nodeDecimalDigits, i3) = GetInt(context, false, i);
                    i = i3;
                }
            }

            // 3. Check for exponent ('e' or 'E'). If found, parse optional sign, then digits.
            //    If no digits, it's an error. We'll rely on double.TryParse to do final check.
            var expMatch = GetLiteralMatch(context, i, "E");
            if (expMatch.NextIndex == i)
                expMatch = GetLiteralMatch(context, i, "e");

            if (expMatch.NextIndex > i)
            {
                hasExp = true;
                i = expMatch.NextIndex;

                // Parse optional '+'/'-' after E/e
                var minusMatch = GetLiteralMatch(context, i, "-");
                if (minusMatch.NextIndex > i)
                {
                    i = minusMatch.NextIndex;
                }
                else
                {
                    var plusMatch = GetLiteralMatch(context, i, "+");
                    if (plusMatch.NextIndex > i)
                        i = plusMatch.NextIndex;
                }

                var (expDigits, nodeExpDigits, i3) = GetInt(context, true, i);
                if (i3 == i)
                {
                    context.SyntaxErrors.Add(new SyntaxErrorData(
                        i,
                        1,
                        $"Invalid exponent: missing digits after E/e"
                    ));
                    return new GetNumberResult(null, null, index);
                }
                i = i3;
            }

            // 4. If we have a decimal or exponent, parse as double
            if (hasDecimal || hasExp)
            {
                if (!double.TryParse(
                        context.Expression.Substring(index, i - index),
                        out var dval
                    ))
                {
                    context.SyntaxErrors.Add(new SyntaxErrorData(
                        index,
                        i - index,
                        $"{context.Expression.Substring(index, i - index)} couldn't be parsed as floating point"
                    ));
                    return new GetNumberResult(null, null, index);
                }

                number = dval;
                parseNode = new ParseNode(ParseNodeType.LiteralDouble, index, i - index);
                return new GetNumberResult(number, parseNode, i);
            }

            // 5. If still no decimal/exponent, check for 'l' (long) suffix
            var lMatch = GetLiteralMatch(context, i, "l");
            if (lMatch.NextIndex > i)
            {
                hasLong = true;
                i = lMatch.NextIndex;
            }

            // 6. Parse integer or long
            if (hasLong)
            {
                if (!long.TryParse(intDigits, out var longVal))
                {
                    context.SyntaxErrors.Add(new SyntaxErrorData(
                        index,
                        intDigits.Length,
                        $"{intDigits} couldn't be parsed to 64-bit integer"
                    ));
                    return new GetNumberResult(null, null, index);
                }

                number = longVal;
                parseNode = new ParseNode(ParseNodeType.LiteralLong, index, i - index);
                return new GetNumberResult(number, parseNode, i);
            }

            // Try 32-bit int first
            if (int.TryParse(intDigits, out var intVal))
            {
                number = intVal;
                parseNode = new ParseNode(ParseNodeType.LiteralInteger, index, i - index);
                return new GetNumberResult(number, parseNode, i);
            }

            // Then 64-bit long
            if (long.TryParse(intDigits, out var lngVal))
            {
                number = lngVal;
                parseNode = new ParseNode(ParseNodeType.LiteralLong, index, i - index);
                return new GetNumberResult(number, parseNode, i);
            }

            // If we still can't parse, fail
            return new GetNumberResult(null, null, index);
        }
    }
}