﻿using System.Text;
using funcscript.core;

namespace funcscript.model
{
    public class SimpleKeyValueCollection: KeyValueCollection
    {
        private KeyValueCollection _parent;
        KeyValuePair<String, object>[] _data;
        Dictionary<String, object> _index;
        public SimpleKeyValueCollection(KeyValuePair<string, object>[] kv)
        :this(null,kv)
        {

        }
        
        public SimpleKeyValueCollection(KeyValueCollection parent, KeyValuePair<string, object>[] kv)
        {
            this.Data = kv;
            this._parent = parent;
        }

        public KeyValuePair<String,object>[] Data 
        { 
            set
            {
                _data=value;
                if(value == null)
                {
                    _index = null;
                    return;
                }
                _index = new Dictionary<string, object>();
                foreach (var kv in value)
                    _index.Add(kv.Key.ToLower(), kv.Value);
            }
        }

        public object Get(string value)
        {
            return _index.GetValueOrDefault(value);
        }

        public KeyValueCollection ParentContext => _parent;

        public bool IsDefined(string value)
        {
            return _index.ContainsKey(value);
        }

        public IList<string> GetAllKeys()
        {
            return this._data.Select(x=>x.Key).ToList();
        }
    }
}
