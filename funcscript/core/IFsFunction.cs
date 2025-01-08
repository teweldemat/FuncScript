using System.Text;
using FuncScript.Model;

namespace FuncScript.Core
{
    
    public interface IFsFunction
    {
        object EvaluateList(KeyValueCollection context,FsList pars);
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
