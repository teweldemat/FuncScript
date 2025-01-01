using funcscript.model;

namespace funcscript.core
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