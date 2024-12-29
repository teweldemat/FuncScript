using System.Collections;
using System.Collections.Specialized;
using funcscript.core;

namespace funcscript.model
{
    public interface KeyValueCollection 
    {
        public object Get(string key);
        public KeyValueCollection ParentContext { get; }
        public bool IsDefined(string key);
        //public IList<KeyValuePair<string, object>> GetAll();
        public IList<String> GetAllKeys();
        public static KeyValueCollection Merge(KeyValueCollection col1, KeyValueCollection col2)
        {
            if (col1 == null && col2 == null)
                return null;
            if (col1 == null)
                return col2;
            if (col2 == null)
                return col1;
            var dict = new OrderedDictionary();
            foreach (var key in col1.GetAllKeys())
                dict[key] = col1.Get(key);
            foreach (var key in col2.GetAllKeys())
            {
                var val = col2.Get(key);
                if (dict.Contains(key))
                {
                    var left = dict[key] as KeyValueCollection;
                    if (left != null && val is KeyValueCollection)
                    {
                        dict[key] = KeyValueCollection.Merge(left, (KeyValueCollection)val);
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

            if (col1.ParentContext != col2.ParentContext)
                throw new error.EvaluationTimeException(
                    "Key value collections from different contexts can't be merged");
            return new SimpleKeyValueCollection(col1.ParentContext, kvs);
        }

        
    }

    public static class KvcExtensions
    {
        public static T ConvertTo<T>(this KeyValueCollection kvc)
        {
            var json = FuncScript.FormatToJson(kvc);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        }
        public static object ConvertTo(this KeyValueCollection kvc,Type t)
        {
            var json = FuncScript.FormatToJson(kvc);
            return Newtonsoft.Json.JsonConvert.DeserializeObject(json,t);
        }
        
    }
}
