namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        static bool IsIdentfierOtherChar(char ch)
        {
            return char.IsLetterOrDigit(ch) || ch == '_';
        }
        
        // Other parser methods can be defined here as needed.
    }
}