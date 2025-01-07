using FuncScript.Block;
using FuncScript.Funcs.Math;
using FuncScript.Model;

namespace FuncScript.Core
{
    public partial class FuncScriptParser
    {
        public static ExpressionBlockResult Parse(ParseContext context)
        {
            return GetRootExpression(context);
        }
    }
}
