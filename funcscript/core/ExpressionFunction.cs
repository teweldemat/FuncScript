using System.Text;
using FuncScript.Model;

namespace FuncScript.Core
{
    public class ExpressionFunction : IFsFunction
    {
        
        private class ParameterDataProvider : KeyValueCollection
        {
            public FsList Pars;
            public KeyValueCollection EvaluationContext;
            public ExpressionFunction ExpressionFunction;
            public KeyValueCollection ParentContext => EvaluationContext;
            public override bool Equals(object obj)
            {
                if (!(obj is KeyValueCollection kvc))
                    return false;
                return this.IsEqualTo(kvc);
            }

            public override int GetHashCode()
            {
                return this.GetKvcHashCode();
            }
            public bool IsDefined(string key)
            {
                return ExpressionFunction.ParamterNameIndex.ContainsKey(key)
                       || EvaluationContext.IsDefined(key);
            }

            public IList<string> GetAllKeys()
            {
                throw new InvalidOperationException();
            }

            public object Get(string name)
            {
                if (ExpressionFunction.ParamterNameIndex.TryGetValue(name, out var index))
                    return Pars[index];
                if (ExpressionFunction._context != null & ExpressionFunction._context.IsDefined(name))
                    return ExpressionFunction._context.Get(name);
                return EvaluationContext.Get(name);
            }
        }

        public ExpressionBlock Expression { get; set; }

        private Dictionary<string, int> ParamterNameIndex;
        private String[] _parameters;
        private object _expressionValue = null;
        private KeyValueCollection _context = null;

        public void SetReferenceProvider(KeyValueCollection context)
        {
            _context = context;
        }
        public ExpressionFunction(String[] pars, ExpressionBlock exp)
        {
            this.Expression = exp;
            this._parameters = pars;
            this.ParamterNameIndex = new Dictionary<String, int>();
            var i = 0;
            foreach (var n in pars)
                this.ParamterNameIndex.Add(n.ToLower(), i++);
        }

        public int MaxParsCount => _parameters.Length;
        public CallType CallType => CallType.Infix;

        public string Symbol => null;

        public object EvaluateList(KeyValueCollection context, FsList pars)
        {
            if (_context == null)
                throw new Error.EvaluationTimeException("Context not set to expression function");
            var clone = this.Expression.CloneExpression();
            clone.SetReferenceProvider( new ParameterDataProvider
            {
                ExpressionFunction = this,
                EvaluationContext = context,
                Pars = pars
            });
            var ret = clone.Evaluate();
            
            return ret;
        }
        
        

        public string ParName(int index)
        {
            return _parameters[index];
        }

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.Symbol);
            sb.Append('(');
            int c = this.MaxParsCount;
            for (int i = 0; i < c; i++)
            {
                if (i > 0)
                    sb.Append(',');
                sb.Append(this.ParName(i));
            }

            sb.Append(')');
            sb.Append("=>");
            sb.Append(this.Expression.AsExpString());
            return sb.ToString();
        }
    }
}
