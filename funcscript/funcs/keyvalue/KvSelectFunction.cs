﻿using funcscript.core;
using funcscript.model;

namespace funcscript.funcs.keyvalue
{
    internal class KvSelectFunction : IFsFunction
    {
        private const int MaxParameters = 2;

        public CallType CallType => CallType.Prefix;

        public string Symbol => "Select";

        public object EvaluateList(FsList pars)
        {
            if (pars.Length != MaxParameters)
                throw new error.TypeMismatchError($"{Symbol} function: Invalid parameter count. Expected {MaxParameters}, but got {pars.Length}");

            var par0 = pars[0];
            var par1 = pars[1];

            if (!(par0 is KeyValueCollection))
                throw new error.TypeMismatchError($"{Symbol} function: The first parameter should be {ParName(0)}");

            if (!(par1 is KeyValueCollection))
                throw new error.TypeMismatchError($"{Symbol} function: The second parameter should be {ParName(1)}");

            var first = (KeyValueCollection)par0;
            var secondKvc = ((KeyValueCollection)par1);
            var second = secondKvc
                .GetAllKeys()
                .Select(k=>KeyValuePair.Create(k,secondKvc.Get(k)))
                .ToList();

            for (int i = 0; i < second.Count; i++)
            {
                if (second[i].Value == null)
                {
                    var key = second[i].Key.ToLower();
                    var value = first.Get(key);
                    second[i] = new KeyValuePair<string, object>(second[i].Key, value);
                }
            }

            return new SimpleKeyValueCollection(second.ToArray());
        }

        public string ParName(int index)
        {
            return index switch
            {
                0 => "Source KVC",
                1 => "Target KVC",
                _ => null
            };
        }
    }
}