using funcscript.model;

namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        public static ParseResult ParseFsTemplate(ParseContext context)
        {
            var result = GetFSTemplate(context, 0);
            result.Expression.SetContext(context.Provider);
            return result;
        }
    }
}