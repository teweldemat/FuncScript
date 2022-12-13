﻿using funcscript.core;
using funcscript.model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace funcscript.funcs.math
{
    public class AddFunction : IFsFunction
    {
        public int MaxParsCount => -1;

        public CallType CallType => CallType.Infix;

        public string Symbol => "+";

        public int Precidence => 100;

        public object Evaluate(IFsDataProvider parent, IParameterList pars)
        {
            bool isNull=true, isInt=false,isLong=false,isDouble=false,isString=false,isList=false;
            bool isKv = false;

            int intTotal = 0;
            long longTotal = 0;
            double doubleTotal = 0;
            StringBuilder stringTotal = null;
            KeyValueCollection kvTotal = null;
            List<object> listTotal= new List<object>();
            int c = pars.Count;
            for(int i=0;i<c;i++)
            {
                var d=pars[i];
                if(isNull)
                {
                    if (d is int)
                    {
                        isNull = false;
                        isInt = true;
                    }
                    else if (d is long)
                    {
                        isNull = false;
                        isLong = true;
                    }
                    else if (d is double)
                    {
                        isNull = false;
                        isDouble = true;
                    }
                    else if (d is String)
                    {
                        isNull = false;
                        isString = true;
                    }
                    else if (d is KeyValueCollection)
                    {
                        isNull = false;
                        isKv = true;
                    }
                    else if (d is FsList)
                    {
                        isNull = false;
                        isList = true;
                    }
                }
                if (isInt)
                {
                    if (d is int)
                    {
                        intTotal += (int)d;
                    }
                    else if (d is long)
                    {
                        isLong = true;
                        isInt = false;
                        longTotal = intTotal;
                    }
                    else if (d is double)
                    {
                        isDouble = true;
                        isInt = false; 
                        doubleTotal = intTotal;
                    }
                    else if (d is String)
                    {
                        isString = true;
                        isInt = false;
                        stringTotal = new StringBuilder(intTotal.ToString());
                    }
                    else if (d is FsList)
                    {
                        isList = true;
                        isInt = false;
                        listTotal = new List<object>(new object[] { intTotal });
                    }
                }
                if (isLong)
                {
                    if(d is int)
                    {
                        longTotal += (long)(int)d;
                    }
                    else if (d is long)
                    {
                        longTotal += (long)d;
                    }
                    else if (d is double)
                    {
                        isDouble = true;
                        isLong= false;
                        doubleTotal = longTotal;
                    }
                    else if (d is String)
                    {
                        isString = true;
                        isLong = false;
                        stringTotal = new StringBuilder(longTotal.ToString());
                    }

                    else if (d is FsList)
                    {
                        isList = true;
                        isLong = false;
                        listTotal = new List<object>(new object[] { longTotal });
                    }

                    else if (d is KeyValueCollection)
                    {
                        throw new error.TypeMismatchError("Keyvalue collection not expected");
                    }

                }
                if (isDouble)
                {
                    if(d is int)
                    {
                        doubleTotal += (double)(int)d;
                    }
                    else if (d is long)
                    {
                        doubleTotal += (double)(long)d;
                    }
                    else if (d is double)
                    {
                        doubleTotal+= (double)d;
                    }
                    else if (d is String)
                    {
                        isString = true;
                        isDouble = false;
                        stringTotal = new StringBuilder(doubleTotal.ToString());
                    }
                    else if (d is FsList)
                    {
                        isList= true;
                        isDouble = false;
                        listTotal = new List<object>(new object[] { longTotal});
                    }

                    else if (d is KeyValueCollection)
                    {
                        throw new error.TypeMismatchError("Keyvalue collection not expected");
                    }
                }
                if (isString)
                {
                    if (d is int)
                    {
                        stringTotal.Append(d.ToString());
                    }
                    else if (d is long)
                    {
                        stringTotal.Append(d.ToString());
                    }
                    else if (d is double)
                    {
                        stringTotal.Append(d.ToString());
                    }
                    else if (d is double)
                    {
                        stringTotal.Append(d.ToString());
                    }
                    else if (d is string)
                    {
                        if (stringTotal == null)
                            stringTotal = new StringBuilder();
                        stringTotal.Append((string)d);
                    }
                    else if (d is FsList)
                    {
                        isList = true;
                        isString= false;
                        listTotal = new List<object>(new object[] { stringTotal.ToString()});
                    }
                    else if (d is KeyValueCollection)
                    {
                        throw new error.TypeMismatchError("Keyvalue collection not expected");
                    }
                }
                if(isKv)
                {
                    var kv = d as KeyValueCollection;
                    if(kv==null)
                        throw new error.TypeMismatchError("Keyvalue collection expected");
                    if (kvTotal == null)
                        kvTotal = kv;
                    else
                        kvTotal = KeyValueCollection.Merge(kvTotal, kv);
                }
                if (isList)
                {
                    if (d is FsList)
                    {
                        foreach(var v in ((FsList)d).Data)
                            listTotal.Add(v);
                    }
                    else
                        listTotal.Add(d);
                }

            }
            if (isList)
                return new FsList(listTotal);
            if (isString)
                return stringTotal.ToString();
            if (isDouble)
                return doubleTotal;
            if (isLong)
                return longTotal;
            if (isInt)
                return intTotal;
            if (isKv)
            {
                return kvTotal;
            }
            return null;
        }

        public string ParName(int index)
        {
            return $"Op {index + 1}";
        }
    }
}
