using funcscript.core;
using funcscript.model;
using System;

namespace funcscript.funcs.misc
{
    public abstract class Fslogger
    {
        public abstract void WriteLine(string text);
        public abstract void Clear();
        
        private static Fslogger _fslogger;

        public static void SetDefaultLogger(Fslogger logger)
        {
            _fslogger = logger;
        }
        public static Fslogger DefaultLogger => _fslogger;

        static Fslogger()
        {
            SetDefaultLogger(new ConsoleLogger());
        }
    }

    public class ConsoleLogger : Fslogger
    {
        public override void WriteLine(string text) => Console.WriteLine(text);
        public override void Clear() => Console.Clear();
    }

    public class LogFunction : IFsFunction
    {
        private const int MaxParameters = 3;

        public CallType CallType => CallType.Infix;

        public string Symbol => "log";

        public object EvaluateList(FsList pars)
        {
            if (pars.Length == 0)
                throw new error.EvaluationTimeException($"{this.Symbol} function: {this.ParName(0)} expected");

            var tag = pars.Length > 1 ? $"({pars[1]?.ToString()})" : "";
            var output = pars.Length > 2 ? (pars[2] is bool ? (bool)pars[2] : false) : true;

            Fslogger.DefaultLogger.WriteLine($"FuncScript: Evaluating {tag}");
            try
            {
                var res = pars[0];
                if (output)
                {
                    Fslogger.DefaultLogger.WriteLine($"FuncScript: Result{tag}:\n{(res == null ? "<null>" : res.ToString())}");
                    Fslogger.DefaultLogger.WriteLine($"End Result {tag}");
                }
                else
                {
                    Fslogger.DefaultLogger.WriteLine($"Done {tag}");
                }
                return res;
            }
            catch (Exception ex)
            {
                Fslogger.DefaultLogger.WriteLine($"FuncScript: Error evaluating {tag}");
                var thisEx = ex;
                while (thisEx != null)
                {
                    Fslogger.DefaultLogger.WriteLine(thisEx.Message);
                    Fslogger.DefaultLogger.WriteLine(thisEx.StackTrace);
                    thisEx = thisEx.InnerException;
                }
                throw;
            }
        }

        public string ParName(int index)
        {
            return index switch
            {
                0 => "expression",
                1 => "tag",
                2 => "output",
                _ => null
            };
        }
    }
}