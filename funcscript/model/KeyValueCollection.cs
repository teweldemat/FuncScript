using System.Collections;
using System.Collections.Specialized;
using funcscript.core;

namespace funcscript.model
{
    public abstract class  KeyValueCollection:IFsDataProvider
    {
        public abstract object Get(string key);
        public abstract IFsDataProvider ParentProvider { get; }
        public abstract bool IsDefined(string key);
        public T ConvertTo<T>()
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(FuncScript.FormatToJson(this));
        }
        public object ConvertTo(Type t)
        {
            var json = FuncScript.FormatToJson(this);
            return Newtonsoft.Json.JsonConvert.DeserializeObject(json,t);
        }
        public abstract IList<KeyValuePair<string, object>> GetAll();
        public override bool Equals(object otherkv)
        {
            var other = otherkv as KeyValueCollection;
            if (other == null)
                return false;
            foreach(var k in other.GetAll())
            {
                if (!this.IsDefined(k.Key.ToLowerInvariant()))
                    return false;
                var thisVal= this.Get(k.Key);
                var otherVal= other.Get(k.Key);
                if (thisVal == null && otherVal == null)
                    return true;
                if (thisVal == null || otherVal == null)
                    return false;
                if (!thisVal.Equals(otherVal))
                    return false;
            }
            return true;
        }
        public static KeyValueCollection Merge(KeyValueCollection col1,KeyValueCollection col2)
        {
            if (col1 == null && col2 == null)
                return null;
            if (col1 == null)
                return col2;
            if (col2 == null)
                return col1;
            var dict = new OrderedDictionary();
            foreach (var kv in col1.GetAll())
                dict[kv.Key] = kv.Value;
            foreach (var kv in col2.GetAll())
            {
                if(dict.Contains(kv.Key))
                {
                    var left = dict[kv.Key] as KeyValueCollection;
                    if (left != null && kv.Value is KeyValueCollection)
                    {
                        dict[kv.Key] = KeyValueCollection.Merge(left,(KeyValueCollection)kv.Value);
                    }
                    else
                        dict[kv.Key] = kv.Value;
                }
                else
                    dict.Add(kv.Key,kv.Value);
            }
            var kvs = new KeyValuePair<string, object>[dict.Count];
            var en = (IDictionaryEnumerator)dict.GetEnumerator();
            int k = 0;
            while (en.MoveNext())
            {
                kvs[k] = new KeyValuePair<string, object>((string)en.Key, en.Value);
                k++;
            }

            if (col1.ParentProvider != col2.ParentProvider)
                throw new error.EvaluationTimeException(
                    "Key value collections from different contexts can't be merged");
            return new SimpleKeyValueCollection(col1.ParentProvider,kvs);
        }
        public override int GetHashCode()
        {
            int hash = 0;
            foreach(var kv in this.GetAll())
            {
                var thisHash = kv.Value == null ? kv.Key.GetHashCode() : HashCode.Combine(kv.Key.GetHashCode(), kv.Value.GetHashCode());
                if (hash == 0)
                    hash = thisHash;
                else
                    hash = HashCode.Combine(hash, thisHash);
            }
            return hash;
        }
    }
}
