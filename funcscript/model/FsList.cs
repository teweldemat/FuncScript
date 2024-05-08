﻿using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace funcscript.model
{
    public abstract class FsList
    {
        public abstract  IEnumerable<object> Data { get; }
        public abstract object this[int index] { get; }
        public abstract int Length { get; }
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            var other = (FsList)obj;
            var otherData = other.Data.ToArray();
            var thisData = this.Data.ToArray();
            if (otherData.Length != thisData.Length)
                return false;
            for(int i=0;i<otherData.Length;i++)
            {
                var val1 = thisData[i];
                var val2 = otherData[i];
                if (val1==null && val2==null)
                    return true;
                if (val1 == null || val2 == null)
                    return false;
                if (!val1.Equals(val2))
                    return false;
            }
            return true;
        }
        public override string ToString()
        {
            if (this.Data == null)
                return "Uninitialized list";
            var sb = new StringBuilder();
            sb.Append("[");
            if (Data.Any())
            {
                var first = true;
                foreach (var d in this.Data)
                {
                    if (first)
                        first = false;
                    else
                        sb.Append(",");
                    if (d == null)
                        sb.Append("null");
                    else
                        sb.Append(d.ToString());

                }
            }
            sb.Append("]");
            return sb.ToString();
        }
        // override object.GetHashCode
        public override int GetHashCode()
        {
            return this.Data.GetHashCode();
        }
        public static bool IsListType(Type t) =>
            t.IsAssignableTo(typeof(System.Collections.IEnumerable)) || t.IsAssignableTo(typeof(System.Collections.IList)) || IsGenericList(t);
        static bool IsGenericList(Type t)
        {
            return t != typeof(byte[]) && t.IsGenericType && (t.GetGenericTypeDefinition().IsAssignableTo(typeof(IList<>))
                || t.GetGenericTypeDefinition().IsAssignableTo(typeof(List<>)));
        }
        
    }

    public class ArrayFsList : FsList
    {
        object[] _data;
        public ArrayFsList(object data)
        {
            if (data == null)
                throw new error.TypeMismatchError("Null can't be converted to list");
            else
            {
                var t = data.GetType();
                if(t.IsAssignableTo(typeof(System.Collections.IList)))
                {
                    var l = (System.Collections.IList)data;
                    _data = new object[l.Count];
                    for (int i = 0; i < l.Count; i++)
                        _data[i] = FuncScript.NormalizeDataType(l[i]);
                }
                else if (t.IsAssignableTo(typeof(System.Collections.IEnumerable)))
                {
                    var l = (System.Collections.IEnumerable)data;
                    var list = new List<object>();
                    foreach (var o in l)
                        list.Add(FuncScript.NormalizeDataType(o));
                    _data = list.ToArray();
                }
                else
                {
                    throw new error.TypeMismatchError($"{t} can't be used as list");
                }
            }
        }

        public override IEnumerable<object> Data => _data;

        public override object this[int index] => (index<0||index>=_data.Length)?null:_data[index];
        public override int Length =>_data.Length;
    }
}
