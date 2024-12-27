using System.Text;
using funcscript.model;

namespace funcscript.core
{
    public interface IFsFunction
    {
        //object Evaluate(KeyValueCollection parent, IParameterList pars);
        object EvaluateList(FsList pars);
        CallType CallType { get; }
        String Symbol { get; }
    }

    public class FunctionAliasAttribute : Attribute
    {
        public string[] Aliaces;
        public FunctionAliasAttribute(params string[] aliaces)
        {
            this.Aliaces = aliaces;
        }
    }

   

    public enum CallType
    {
        Infix,
        Prefix,
        Dual
    }
}
