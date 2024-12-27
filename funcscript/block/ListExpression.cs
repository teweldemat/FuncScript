using System.Collections;
using System.Runtime.CompilerServices;
using funcscript.core;
using funcscript.model;
using System.Text;
using Newtonsoft.Json.Serialization;

namespace funcscript.block
{
    public class ListExpression:ExpressionBlock,FsList
    {
       
        public ExpressionBlock[] ValueExpressions;

       
        public override object Evaluate( )
        {
            var lst = ValueExpressions.Select(x => x.Evaluate()).ToArray();
            return new ArrayFsList(lst);
        }
        public override IList<ExpressionBlock> GetChilds()
        {
            var ret = new List<ExpressionBlock>();
            ret.AddRange(this.ValueExpressions);
            return ret;
        }

        public object this[int index] =>index<0 || index>=this.ValueExpressions.Length?null:this.ValueExpressions[index].Evaluate();

        public int Length => this.ValueExpressions.Length;
        public IEnumerator<object> GetEnumerator()
        {
            foreach (var expr in ValueExpressions)
            {
                yield return expr.Evaluate();
            }
        }

        

        public override string ToString()
        {
            return "list";
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        public override string AsExpString()
        {
            var sb = new StringBuilder();
            sb.Append("[");
            
            foreach (var val in this.ValueExpressions)
            {
                sb.Append($"{val.AsExpString()},");
            }
            sb.Append("]");
            return sb.ToString();
        }
    }
}
