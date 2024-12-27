namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        static int GetExpression(IFsDataProvider parseContext, String exp, int index, out ExpressionBlock prog,
            out ParseNode parseNode, List<SyntaxErrorData> serrors)
        {
            var i = GetInfixExpression(parseContext, exp, index, out prog, out parseNode, serrors);
            if (i > index)
                return i;

            return index;
        }
        
    }
}