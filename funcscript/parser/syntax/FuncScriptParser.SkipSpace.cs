namespace funcscript.core
{
    public partial class FuncScriptParser
    {
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
    }
}
