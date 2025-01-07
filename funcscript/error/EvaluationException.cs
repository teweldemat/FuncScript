using FuncScript.Core;

namespace FuncScript.Error
{
    public class EvaluationException : Exception
    {
        public int Pos;
        public int Len;
        public EvaluationException(int i, int l, Exception innerException)
            : this(null, i, l, innerException)
        {

        }
        public EvaluationException(string message, int i, int l, Exception innerException)
            : base(message, innerException)
        {
            this.Pos = i;
            this.Len = l;
        }
    }
}
