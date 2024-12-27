using funcscript.model;
namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        static int GetInt(KeyValueCollection provider, string exp, bool allowNegative, int index, out string intVal, out ParseNode parseNode)
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
    }
}