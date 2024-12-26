using funcscript.core;
using funcscript.model;
using System;
using System.Text;

namespace funcscript.funcs.math
{
    public class TemplateMergeMergeFunction : IFsFunction
    {
        public const string SYMBOL = "_templatemerge";
        public int MaxParsCount => -1;

        public CallType CallType => CallType.Infix;

        public string Symbol => SYMBOL;

        public int Precidence => 100;

        void MergeList(StringBuilder sb, FsList list) 
        {
            if (list == null)
                return;
            foreach (var o in list)
            {
                if (o is FsList)
                    MergeList(sb, (FsList)o);
                else
                    sb.Append(o == null ? "" : o.ToString());
            }
        }

        public object Evaluate(IFsDataProvider parent, IParameterList pars)
        {
            StringBuilder sb = new StringBuilder();
            int c = pars.Count;
            for (int i = 0; i < c; i++)
            {
                var o = pars.GetParameter(parent, i);
                if (o is FsList)
                    MergeList(sb, (FsList)o);
                else
                    sb.Append(o == null ? "" : o.ToString());
            }
            return sb.ToString();
        }

        public string ParName(int index)
        {
            return $"Op {index + 1}";
        }
    }
}