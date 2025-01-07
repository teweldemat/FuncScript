using System.Text;
using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Block
{
    public class LiteralBlock : ExpressionBlock
    {
        public object Value;
        public LiteralBlock(object val)
        {
            Value = val;
        }

        public override string AsExpString()
        {
            var sb = new StringBuilder();
            FuncScript.Format(sb, Value, null, true, false);
            return sb.ToString();
        }

        public override object Evaluate()
        {
            return Value;
        }
        public override IList<ExpressionBlock> GetChilds()
        {
            return new ExpressionBlock[0];
        }
        public override string ToString()
        {
            if (Value == null)
                return "";
            return Value.ToString();
        }

        public override void SetReferenceProvider(KeyValueCollection provider)
        {
            if (Value is ExpressionFunction exp)
            {
                exp.SetReferenceProvider(provider);
            }
        }

        public override ExpressionBlock CloneExpression()
        {
            return new LiteralBlock(this.Value);
        }
    }
}
