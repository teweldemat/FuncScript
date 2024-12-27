using funcscript.block;
using funcscript.funcs.math;

namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        public static ExpressionBlock ParseFsTemplate(IFsDataProvider context, String exp, out ParseNode parseNode,
            List<SyntaxErrorData> serrors)
        {
            var i = GetFSTemplate(context, exp, 0, out var block, out parseNode, serrors);
            block.Provider = context;
            return block;
        }

        public static ExpressionBlock ParseFsTemplate(IFsDataProvider context, String exp,
            List<SyntaxErrorData> serrors)
        {
            return ParseFsTemplate(context, exp, out var node, serrors);
        }
    }
}