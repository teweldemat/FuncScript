using FuncScript.Core;
using FuncScript.Model;
using System;
using System.Text;

namespace FuncScript.Funcs.Math
{
    public class TemplateMergeMergeFunction : IFsFunction
    {
        public const string SYMBOL = "_templatemerge";

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
            if (pars == null)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH, $"{this.Symbol}: parameters cannot be null");

            StringBuilder sb = new StringBuilder();
            int c = pars.Length;
            for (int i = 0; i < c; i++)
            {
                var o = pars[i];
                if (o is FsList)
                {
                    MergeList(sb, (FsList)o);
                }
                else if (o == null)
                {
                    sb.Append("");
                }
                else
                {
                    sb.Append(o.ToString());
                }
            }
            return sb.ToString();
        }

        public string ParName(int index)
        {
            return $"Op {index + 1}";
        }
    }
}
