namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        static bool IsIdentfierOtherChar(char ch)
        {
            return char.IsLetterOrDigit(ch) || ch == '_';
        }
    }
}