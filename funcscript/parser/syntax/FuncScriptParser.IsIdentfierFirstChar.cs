namespace FuncScript.Core
{
    public partial class FuncScriptParser
    {
        static bool IsIdentifierFirstChar(char ch)
        {
            return char.IsLetter(ch) || ch == '_';
        }
    }
}
