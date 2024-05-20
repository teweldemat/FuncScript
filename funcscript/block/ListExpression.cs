﻿using funcscript.core;
using funcscript.model;
using System.Text;
using Newtonsoft.Json.Serialization;

namespace funcscript.block
{
    public class ListExpression:ExpressionBlock
    {
       
        public ExpressionBlock[] ValueExpressions;

       
        public override (object,CodeLocation) Evaluate(IFsDataProvider provider,List<Action> connectionActions )
        {
            var lst = ValueExpressions.Select(x => x.Evaluate(provider,connectionActions).Item1).ToArray();
            return (new ArrayFsList(lst),this.CodeLocation);
        }
        public override IList<ExpressionBlock> GetChilds()
        {
            var ret = new List<ExpressionBlock>();
            ret.AddRange(this.ValueExpressions);
            return ret;
        }
        public override string ToString()
        {
            return "list";
        }
        public override string AsExpString(IFsDataProvider provider)
        {
            var sb = new StringBuilder();
            sb.Append("[");
            
            foreach (var val in this.ValueExpressions)
            {
                sb.Append($"{val.AsExpString(provider)},");
            }
            sb.Append("]");
            return sb.ToString();
        }
    }
}
