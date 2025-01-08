using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace FuncScript.Model
{
    public interface FsList : IEnumerable<object>
    {
        public abstract object this[int index] { get; }
        public abstract int Length { get; }
        public abstract IEnumerator<object> GetEnumerator();
        
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
                throw new Error.TypeMismatchError("Null can't be converted to list");
            else
            {
                var t = data.GetType();
                if(t.IsAssignableTo(typeof(System.Collections.IList)))
                {
                    var l = (System.Collections.IList)data;
                    _data = new object[l.Count];
                    for (int i = 0; i < l.Count; i++)
                        _data[i] = Helpers.NormalizeDataType(l[i]);
                }
                else if (t.IsAssignableTo(typeof(System.Collections.IEnumerable)))
                {
                    var l = (System.Collections.IEnumerable)data;
                    var list = new List<object>();
                    foreach (var o in l)
                        list.Add(Helpers.NormalizeDataType(o));
                    _data = list.ToArray();
                }
                else
                {
                    throw new Error.TypeMismatchError($"{t} can't be used as list");
                }
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        public object this[int index] => (index<0||index>=_data.Length)?null:_data[index];
        public int Length =>_data.Length;
        public IEnumerator<object> GetEnumerator() => ((System.Collections.Generic.IEnumerable<object>)_data).GetEnumerator();
    }
}
