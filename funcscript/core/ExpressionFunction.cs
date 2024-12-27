using System.Text;
using funcscript.model;

namespace funcscript.core
{
    public class ExpressionFunction : IFsFunction
    {
        
        private class ParameterDataProvider : IFsDataProvider
        {
            public FsList pars;
            public IFsDataProvider EvaluationContext;
            public ExpressionFunction expressionFunction;
            public IFsDataProvider ParentContext => EvaluationContext;
            public bool IsDefined(string key)
            {
                return expressionFunction.ParamterNameIndex.ContainsKey(key)
                       || EvaluationContext.IsDefined(key);
            }

            public object Get(string name)
            {
                if (expressionFunction.ParamterNameIndex.TryGetValue(name, out var index))
                    return pars[index];
                if (expressionFunction._context!=null & expressionFunction._context.IsDefined(name))
                    return expressionFunction._context.Get(name);
                return EvaluationContext.Get(name);
            }
        }

        public ExpressionBlock Expression { get; set; }

        private Dictionary<string, int> ParamterNameIndex;
        private String[] _parameters;
        private object _expressionValue = null;
        private IFsDataProvider _context = null;

        public void SetContext(IFsDataProvider context)
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

        public object EvaluateList(FsList pars)
        {
            if (_context == null)
                throw new error.EvaluationTimeException("Context not set to expression function");
            var clone = this.Expression.CloneExpression();
            clone.SetContext( new ParameterDataProvider
            {
                expressionFunction = this,
                EvaluationContext = _context,
                pars = pars
            });
            var ret= clone.Evaluate();
            return ret;
        }
        public object EvaluateWithContext(IFsDataProvider context, FsList pars)
        {
            var clone = this.Expression.CloneExpression();
            clone.SetContext( new ParameterDataProvider
            {
                expressionFunction = this,
                EvaluationContext = context,
                pars = pars
            });
            var ret= clone.Evaluate();
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