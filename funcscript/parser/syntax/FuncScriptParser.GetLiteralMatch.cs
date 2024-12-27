using funcscript.block;
using funcscript.funcs.math;

namespace funcscript.core
{
    public partial class FuncScriptParser
    {

        static public int GetLiteralMatch(string exp, int index, params string[] keyWord)
        {
            return GetLiteralMatch(exp, index, keyWord, out var matched);
        }

        static public int GetLiteralMatch(string exp, int index, string[] keyWord, out string matched)
        {
            if (exp == null)
            {
                throw new ArgumentNullException(nameof(exp), "The input expression cannot be null.");
            }

            foreach (var k in keyWord)
            {
                bool matchFound = true;
                if (index + k.Length <= exp.Length)
                {
                    for (int i = 0; i < k.Length; i++)
                    {
                        if (char.ToLowerInvariant(exp[index + i]) != char.ToLowerInvariant(k[i]))
                        {
                            matchFound = false;
                            break;
                        }
                    }

                    if (matchFound)
                    {
                        matched = k.ToLowerInvariant();
                        return index + k.Length;
                    }
                }
            }

            matched = null;
            return index;
        }
    }
}