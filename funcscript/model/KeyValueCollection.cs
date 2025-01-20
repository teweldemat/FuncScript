using System.Collections;
using System.Collections.Specialized;
using FuncScript.Core;

namespace FuncScript.Model
{
    public interface KeyValueCollection
    {
        public object Get(string key);
        public KeyValueCollection ParentContext { get; }

        public bool IsDefined(string key);

        //public IList<KeyValuePair<string, object>> GetAll();
        public IList<String> GetAllKeys();

        static KeyValueCollection MergeInternal(KeyValueCollection col1, KeyValueCollection col2,
            bool assertSameContext)
        {
            if (col1 == null && col2 == null)
                return null;
            if (col1 == null)
                return col2;
            if (col2 == null)
                return col1;
            if (assertSameContext && col1.ParentContext != col2.ParentContext)
                throw new Error.EvaluationTimeException(
                    "Key value collections from different contexts can't be merged");

            var dict = new OrderedDictionary();
            foreach (var key in col1.GetAllKeys())
                dict[key] = col1.Get(key);
            foreach (var key in col2.GetAllKeys())
            {
                var val = col2.Get(key);
                if (dict.Contains(key))
                {
                    var left = dict[key] as KeyValueCollection;
                    if (left != null && val is KeyValueCollection right)
                    {
                        dict[key] = KeyValueCollection.MergeInternal(left, right, false);
                    }
                    else
                        dict[key] = val;
                }
                else
                    dict.Add(key, val);
            }

            var kvs = new KeyValuePair<string, object>[dict.Count];
            var en = (IDictionaryEnumerator)dict.GetEnumerator();
            int k = 0;
            while (en.MoveNext())
            {
                kvs[k] = new KeyValuePair<string, object>((string)en.Key, en.Value);
                k++;
            }

            return new SimpleKeyValueCollection(col1.ParentContext, kvs);
        }

        public static KeyValueCollection Merge(KeyValueCollection col1, KeyValueCollection col2)
        {
            return MergeInternal(col1, col2, true);
        }
    }

    public static class KvcExtensions
    {
        public static T ConvertTo<T>(this KeyValueCollection kvc)
        {
            var json = Helpers.FormatToJson(kvc);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        }

        public static object ConvertTo(this KeyValueCollection kvc, Type t)
        {
            var json = Helpers.FormatToJson(kvc);
            return Newtonsoft.Json.JsonConvert.DeserializeObject(json, t);
        }

        public static bool IsEqualTo(this KeyValueCollection kvc1, KeyValueCollection kvc2)
        {
            if (kvc2 == null)
                return false;
            var keys1 = kvc1.GetAllKeys();
            var keys2 = kvc2.GetAllKeys();
            if (keys1.Count != keys2.Count)
                return false;
            foreach (var k in keys1)
            {
                var val1 = kvc1.Get(k);
                var val2 = kvc2.Get(k);
                if (val1 == null && val2 == null)
                    return true;
                if (val1 == null || val2 == null)
                    return false;
                if (!val1.Equals(val2))
                    return false;
            }
            return true;
        }

        public static int GetKvcHashCode(this KeyValueCollection kvc)
        {
            if (kvc == null)
            {
                throw new ArgumentNullException(nameof(kvc));
            }

            var keys = kvc.GetAllKeys();
            int hash = 17; // A prime number to start hash calculation

            foreach (var key in keys)
            {
                var value = kvc.Get(key);
                hash = hash * 31 + (key?.GetHashCode() ?? 0); // Combine key hash
                hash = hash * 31 + (value?.GetHashCode() ?? 0); // Combine value hash
            }

            return hash;
        }
    }
}

