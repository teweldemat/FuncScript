using FuncScript.Core;
namespace FuncScript.Model
{
    internal class ByteArray 
    {
        public byte[] Bytes;
    }

    internal class DelegateFunction : IFsFunction
    {
        private Delegate f;
        System.Reflection.ParameterInfo[] _pars;
        public DelegateFunction(Delegate f)
        {
            this.f = f;
            var m = f.Method;
            if (m.ReturnType == typeof(void))
                throw new Error.TypeMismatchError("Delegate with no return is not supported");
            _pars = m.GetParameters();
            foreach(var p in _pars)
            {
                if(p.IsOut)
                    throw new Error.TypeMismatchError($"Delegate with output parameters not supported. Par:{p.Name}");
            }
        }

        public int MaxParsCount => _pars.Length;

        public CallType CallType => CallType.Infix;

        public string Symbol => throw new NotSupportedException();

        public object EvaluateList(KeyValueCollection context, FsList pars)
        {
            return this.f.DynamicInvoke(pars.ToArray());
        }

        public string ParName(int index)
        {
            throw new NotImplementedException();
        }
    }
}
