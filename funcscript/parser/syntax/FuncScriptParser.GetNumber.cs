using funcscript.block;
using funcscript.funcs.math;
using funcscript.model;
namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        public record GetNumberResult(object Number, ParseNode ParseNode, int NextIndex);

        static GetNumberResult GetNumber(ParseContext context, int index)
        {
            ParseNode parseNode = null;
            var hasDecimal = false;
            var hasExp = false;
            var hasLong = false;
            object number = null;
            int i = index;
            var(intDigits,nodeDigits, i2) = GetInt(context, true, i);
            if (i2 == i)
                return new GetNumberResult(null, null, index);
            i = i2;

            i2 = GetLiteralMatch(context, i, ".").NextIndex;
            if (i2 > i)
                hasDecimal = true;
            i = i2;
            if (hasDecimal)
            {
                (var decimalDigits, var nodeDecimlaDigits, i) = GetInt(context, false, i);
            }

            i2 = GetLiteralMatch(context, i, "E").NextIndex;
            if (i2 > i)
                hasExp = true;
            i = i2;
            string expDigits = null;
            ParseNode nodeExpDigits;
            if (hasExp)
                (expDigits,nodeExpDigits, i) = GetInt(context, true, i);

            if (!hasDecimal)
            {
                i2 = GetLiteralMatch(context, i, "l").NextIndex;
                if (i2 > i)
                    hasLong = true;
                i = i2;
            }

            if (hasDecimal)
            {
                if (!double.TryParse(context.Expression.Substring(index, i - index), out var dval))
                {
                    context.SyntaxErrors.Add(new SyntaxErrorData(index, i - index,
                        $"{context.Expression.Substring(index, i - index)} couldn't be parsed as floating point"));
                    return new GetNumberResult(null, null, index);
                }

                number = dval;
                parseNode = new ParseNode(ParseNodeType.LiteralDouble, index, i - index);
                return new GetNumberResult(number, parseNode, i);
            }

            if (hasExp)
            {
                if (!int.TryParse(expDigits, out var e) || e < 0)
                {
                    context.SyntaxErrors.Add(new SyntaxErrorData(index, expDigits == null ? 0 : expDigits.Length,
                        $"Invalid exponentional {expDigits}"));
                    return new GetNumberResult(null, null, index);
                }

                var maxLng = long.MaxValue.ToString();
                if (maxLng.Length + 1 < intDigits.Length + e)
                {
                    context.SyntaxErrors.Add(new SyntaxErrorData(index, expDigits.Length,
                        $"Exponential {expDigits} is out of range"));
                    return new GetNumberResult(null, null, index);
                }

                intDigits = intDigits + new string('0', e);
            }

            long longVal;

            if (hasLong)
            {
                if (!long.TryParse(intDigits, out longVal))
                {
                    context.SyntaxErrors.Add(new SyntaxErrorData(index, expDigits.Length,
                        $"{intDigits} couldn't be parsed to 64bit integer"));
                    return new GetNumberResult(null, null, index);
                }

                number = longVal;
                parseNode = new ParseNode(ParseNodeType.LiteralLong, index, i - index);
                return new GetNumberResult(number, parseNode, i);
            }

            if (int.TryParse(intDigits, out var intVal))
            {
                number = intVal;
                parseNode = new ParseNode(ParseNodeType.LiteralInteger, index, i - index);
                return new GetNumberResult(number, parseNode, i);
            }

            if (long.TryParse(intDigits, out longVal))
            {
                number = longVal;
                parseNode = new ParseNode(ParseNodeType.LiteralLong, index, i - index);
                return new GetNumberResult(number, parseNode, i);
            }

            return new GetNumberResult(null, null, index);
        }
    }
}