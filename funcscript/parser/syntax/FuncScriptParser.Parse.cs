using funcscript.block;
using funcscript.funcs.math;
using funcscript.model;

namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        public static ExpressionBlockResult Parse(ParseContext context)
        {
            return GetRootExpression(context);
        }
    }
}