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

    public static class FsListExtensions
    {
        public static bool IsEqualTo(this FsList list1, FsList list2)
        {
            if (list1 == null && list2 == null) return true;
            if (list1 == null || list2 == null) return false;
            if (list1.Length != list2.Length) return false;

            for (int i = 0; i < list1.Length; i++)
            {
                var item1 = list1[i];
                var item2 = list2[i];
                if (!Equals(item1, item2))
                {
                    return false;
                }
            }

            return true;
        }

        public static int GetListHashCode(this FsList list)
        {
            if (list == null) return 0;

            int hash = 17;
            foreach (var item in list)
            {
                hash = hash * 31 + (item?.GetHashCode() ?? 0);
            }

            return hash;
        }
    }
    public class ArrayFsList : FsList
    {
        object[] _data;
        public override bool Equals(object obj)
        {
            return this.IsEqualTo(obj as FsList);
        }

        public override int GetHashCode()
        {
            return this.GetListHashCode();
        }

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
