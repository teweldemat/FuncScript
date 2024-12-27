using System.Collections;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using funcscript.core;
using funcscript.error;
using funcscript.model;
using System.Text;

namespace funcscript.block
{
    public class FunctionCallExpression : ExpressionBlock
    {
        public ExpressionBlock Function;
        public ExpressionBlock[] Parameters;


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


        private IFsDataProvider _context = null;

        public override void SetContext(IFsDataProvider provider)
        {
            _context = provider;
            this.Function.SetContext(provider);
            foreach (var p in Parameters)
                p.SetContext(provider);
        }


        public override object Evaluate()
        {
            var func = Function.Evaluate();
            var paramList = new FuncParameterList
            {
                parent = this
            };
            if (func is IFsFunction fn)
            {
                try
                {
                    if (func is ExpressionFunction expfn)
                    {
                        return expfn.EvaluateWithContext(_context, paramList);
                    }

                    return fn.EvaluateList(paramList);
                }
                catch (error.EvaluationException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new error.EvaluationException(this.CodePos, this.CodeLength, ex);
                }

            }
            else if (func is FsList)
            {
                var index = paramList[0];
                object ret;
                if (index is int)
                {
                    var i = (int)index;
                    var lst = (FsList)func;
                    if (i < 0 || i >= lst.Length)
                        ret = null;
                    else
                        ret = lst[i];
                }
                else
                    ret = null;

                return ret;
            }
            else if (func is KeyValueCollection collection)
            {
                var index = paramList[0];

                if (index is string key)
                {
                    var kvc = collection;
                    var value = kvc.Get(key.ToLower());
                    return value;
                }

                return null;
            }

            throw new EvaluationException(this.CodePos, this.CodeLength,
                new TypeMismatchError(
                    $"Function part didn't evaluate to a function or a list. {FuncScript.GetFsDataType(func)}"));
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
            string infix = null;
            var f = this.Function.Evaluate() as IFsFunction;
            if (f != null && f.CallType == CallType.Prefix)
            {
                infix = f.Symbol;
            }

            var sb = new StringBuilder();
            if (infix == null)
            {
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
            }
            else
            {
                if (Parameters.Length > 0)
                {
                    sb.Append(this.Parameters[0].AsExpString());
                    for (int i = 1; i < Parameters.Length; i++)
                    {
                        sb.Append($" {infix} ");
                        sb.Append(this.Parameters[i].AsExpString());
                    }
                }
            }

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
