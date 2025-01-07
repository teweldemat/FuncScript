using FuncScript.Model;

namespace FuncScript.Core
{
    public partial class FuncScriptParser
    {
        public static ExpressionBlockResult ParseFsTemplate(ParseContext context)
        {
            var result = GetFSTemplate(context, 0);
            // Remove the call to SetContext, as it is no longer needed
            return result;
        }
    }
}
