using System.Data;
using System.Text;
using funcscript.core;
using funcscript.error;
using funcscript.model;

namespace funcscript.nodes;
public class SigSequenceNode:SignalSourceDelegate,SignalListenerDelegate
    {
        public FsList Items;
        private SignalSinkInfo _sink = new SignalSinkInfo();
        public void SetSource(object listener, object catcher)=>
            _sink.SetSink(listener,catcher);

        public void Activate()
        {
            if(Items==null || Items.Length==0)
                return;
            foreach (var item in Items)
            {
                object sink;
                object fault=null;
                try
                {
                    if (item is FsList pair)
                    {
                        if (pair.Length != 2)
                            throw new EvaluationTimeException(
                                "Exactly two elemeents, normal and listners  are expected");
                        sink = pair[0];
                        fault = pair[1];
                    }
                    else
                    {
                        sink = item;
                    }
                    if (FuncScript.Dref(sink,true) is SignalListenerDelegate s)
                            s.Activate();
                    
                }
                catch (Exception ex)
                {
                    if (FuncScript.Dref(fault,true) is SignalListenerDelegate s)
                    {
                        var x = ex;
                        var sb = new StringBuilder();
                        while (x!=null)
                        {
                            if (sb.Length > 0)
                                sb.Append("\n");
                            sb.Append(x.Message+"\n"+ex.StackTrace);
                            x = x.InnerException;
                        }
                        SignalSinkInfo.ThreadErrorObjects[System.Threading.Thread.CurrentThread.ManagedThreadId] = new ObjectKvc(
                            new SignalSinkInfo.ErrorObject()
                            {
                                Message = sb.ToString(),
                                ErrorType = ex.GetType().ToString(),
                            });
                        s.Activate();
                        SignalSinkInfo.ThreadErrorObjects.TryRemove(System.Threading.Thread.CurrentThread.ManagedThreadId, out _);
                    }
                    else
                    {
                        Console.WriteLine($"Unhandled: {ex.Message}");
                        throw;
                    }
                }
            }
            _sink.Signal();
        }
    }
public class SigSequenceFunction : IFsFunction, IFsDref
{
    public const string SYMBOL = "SigSequence";
    public string Symbol => SYMBOL;
    public int MaxParsCount => -1;  // Variable number of parameters
    public CallType CallType => CallType.Prefix;
    public int Precidence => 0;

    public object Evaluate(IFsDataProvider parent, IParameterList pars)
    {
        var parBuilder = new CallRefBuilder(this, parent, pars);
        var par0 = parBuilder.GetParameter(0);
        
        if (par0 is ValueReferenceDelegate)
            return parBuilder.CreateRef();

        if (!(par0 is FsList list))
            throw new TypeMismatchError($"List of signal sinks expected, found {(par0 == null ? "null" : par0.GetType().ToString())}.");

        return CreateSigSequenceNode(list);
    }

    public object DrefEvaluate(IParameterList pars)
    {
        var list = FuncScript.Dref(pars.GetParameter(null, 0)) as FsList;
        return CreateSigSequenceNode(list);
    }

    

    private bool IsDeferredPair(object item)
    {
        if (item is FsList l && l.Length == 2)
        {
            return l[0] is ValueReferenceDelegate || l[1] is ValueReferenceDelegate;
        }
        return false;
    }

    private object CreateSigSequenceNode(FsList list)
    {
        return new SigSequenceNode {Items = list};
    }

    public string ParName(int index)
    {
        return $"Expression {index + 1}";
    }
}