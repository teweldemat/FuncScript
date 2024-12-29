using funcscript.core;
using funcscript.model;
using System;
using System.Text;

namespace funcscript.funcs.math
{
    public class TemplateMergeMergeFunction : IFsFunction
    {
        public const string SYMBOL = "_templatemerge";
        private const int MAX_PARS_COUNT = -1; // Replaced MaxParsCount

        public CallType CallType => CallType.Infix;

        public string Symbol => SYMBOL;

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

        public object EvaluateList(FsList pars)
        {
            StringBuilder sb = new StringBuilder();
            int c = pars.Length;
            for (int i = 0; i < c; i++)
            {
                var o = pars[i];
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