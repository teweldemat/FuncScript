using System.Collections;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using FuncScript.Core;
using FuncScript.Error;
using FuncScript.Model;
using System.Text;

namespace FuncScript.Block
{
    public class FunctionCallExpression : ExpressionBlock
    {
        public ExpressionBlock Function;
        public ExpressionBlock[] Parameters;
        private object _result = null;
        private bool _evaluated = false;

        class FuncParameterList : FsList
        {
            public FunctionCallExpression parent;

            public object this[int index] => index < 0 || index >= parent.Parameters.Length
                ? null
                : parent.Parameters[index].Evaluate();

            public int Length => parent.Parameters.Length;

            // Implementing the GetEnumerator method to return an enumerator for the parameters
            public IEnumerator<object> GetEnumerator()
            {
                foreach (var parameter in parent.Parameters)
                {
                    yield return parameter.Evaluate();
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public int Count => Parameters.Length;

        private KeyValueCollection _context = null;

        public override void SetReferenceProvider(KeyValueCollection provider)
        {
            _context = provider;
            this.Function.SetReferenceProvider(provider);
            foreach (var p in Parameters)
                p.SetReferenceProvider(provider);
        }

        public override object Evaluate()
        {
            if (_evaluated)
                return _result;
            var func = Function.Evaluate();
            var paramList = new FuncParameterList
            {
                parent = this
            };
            object res;
            if (func is IFsFunction fn)
            {
                try
                {
                    
                    res = fn.EvaluateList(_context,paramList);
                }
                catch (Error.EvaluationException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new Error.EvaluationException(this.CodePos, this.CodeLength, ex);
                }

            }
            else if (func is FsList)
            {
                var index = paramList[0];
                if (index is int)
                {
                    var i = (int)index;
                    var lst = (FsList)func;
                    if (i < 0 || i >= lst.Length)
                        res = null;
                    else
                        res = lst[i];
                }
                else
                    res = null;
            }
            else if (func is KeyValueCollection collection)
            {
                var index = paramList[0];

                if (index is string key)
                {
                    var kvc = collection;
                    res = kvc.Get(key.ToLower());
                }
                else
                    res = null;
            }
            else
                throw new EvaluationException(this.CodePos, this.CodeLength,
                new TypeMismatchError(
                    $"Function part didn't evaluate to a function or a list. {Helpers.GetFsDataType(func)}"));

            _evaluated = true;
            _result = res;
            return res;
        }

        public override IList<ExpressionBlock> GetChilds()
        {
            var ret = new List<ExpressionBlock>();
            ret.Add(this.Function);
            ret.AddRange(this.Parameters);
            return ret;
        }

        public override string ToString()
        {
            return "function";
        }

        public override string AsExpString()
        {
            var sb = new StringBuilder();
            sb.Append(this.Function.AsExpString());
            sb.Append("(");
            if (Parameters.Length > 0)
            {
                sb.Append(this.Parameters[0].AsExpString());
                for (int i = 1; i < Parameters.Length; i++)
                {
                    sb.Append(",");
                    sb.Append(this.Parameters[i].AsExpString());
                }
            }

            sb.Append(")");

            return sb.ToString();
        }

        public override ExpressionBlock CloneExpression()
        {
            return new FunctionCallExpression
            {
                Function = this.Function.CloneExpression(),
                Parameters = this.Parameters.Select(p => p.CloneExpression()).ToArray()
            };
        }
    }
}
