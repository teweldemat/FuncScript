using funcscript.block;
using funcscript.funcs.math;
using funcscript.model;

namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        public static ExpressionBlock ParseFsTemplate(KeyValueCollection provider, String exp, out ParseNode parseNode,
            List<SyntaxErrorData> syntaxErrors)
        {
            var i = GetFSTemplate(provider, exp, 0, out var expBlock, out parseNode, syntaxErrors);
            expBlock.SetContext(provider);
            return expBlock;
        }

        public static ExpressionBlock ParseFsTemplate(KeyValueCollection provider, String exp,
            List<SyntaxErrorData> syntaxErrors)
        {
            return ParseFsTemplate(provider, exp, out var parseNode, syntaxErrors);
        }
    }
}