using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Block
{
    public class NullExpressionBlock : ExpressionBlock
    {
        public override object Evaluate()
        {
            return null;
        }
        public override IList<ExpressionBlock> GetChilds()
        {
            return new ExpressionBlock[0];
        }
        public override string AsExpString()
        {
            return "null";
        }
        public override void SetReferenceProvider(KeyValueCollection provider)
        {
            
        }

        public override ExpressionBlock CloneExpression()
        {
            return new NullExpressionBlock()
            {
                CodePos = this.CodePos,
                CodeLength = this.CodeLength
            };
        }
    }
}
