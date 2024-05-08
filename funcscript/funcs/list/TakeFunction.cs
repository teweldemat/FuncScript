﻿using funcscript.core;
using funcscript.model;

namespace funcscript.funcs.list
{
    public class TakeFunction : IFsFunction
    {
        public int MaxParsCount => 2;

        public CallType CallType => CallType.Prefix;

        public string Symbol => "Take";

        public int Precidence => 0;

        class ParList : IParameterList
        {
            public object X;
            public object N;

            public int Count => 2;

            public object GetParameter(IFsDataProvider provider, int index)
            {
                return index switch
                {
                    0 => X,
                    1 => N,
                    _ => null,
                };
            }
        }

        public object Evaluate(IFsDataProvider parent, IParameterList pars)
        {
            if (pars.Count != this.MaxParsCount)
                throw new error.EvaluationTimeException(
                    $"{this.Symbol} function: Invalid parameter count. Expected {this.MaxParsCount}, but got {pars.Count}");

            var par0 = pars.GetParameter(parent, 0);
            var par1 = pars.GetParameter(parent, 1);

            if (par0 == null)
                return null;

            if (!(par0 is FsList))
                throw new error.TypeMismatchError(
                    $"{this.Symbol} function: The first parameter should be {this.ParName(0)}");

            if (!(par1 is int))
                throw new error.TypeMismatchError(
                    $"{this.Symbol} function: The second parameter should be {this.ParName(1)}");

            var lst = (FsList)par0;
            int n = (int)par1;

            if (n <= 0)
                return new ArrayFsList(new object[] { });

            if (n > lst.Length)
                n = lst.Length;

            object[] newArray = new object[n];
            Array.Copy(lst.Data.ToArray(), 0, newArray, 0, n);

            return new ArrayFsList(newArray);
        }

        public string ParName(int index)
        {
            switch (index)
            {
                case 0:
                    return "List";
                case 1:
                    return "Number";
                default:
                    return "";
            }
        }
    }
}
