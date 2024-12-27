﻿using System.Text;
using funcscript.core;

namespace funcscript.model
{
    public class SimpleKeyValueCollection: KeyValueCollection
    {
        private IFsDataProvider _parent;
        KeyValuePair<String, object>[] _data;
        Dictionary<String, object> _index;
        public SimpleKeyValueCollection(KeyValuePair<string, object>[] kv)
        :this(null,kv)
        {

        }
        
        public SimpleKeyValueCollection(IFsDataProvider parent, KeyValuePair<string, object>[] kv)
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

        public IFsDataProvider ParentProvider => _parent;

        public bool IsDefined(string value)
        {
            return _index.ContainsKey(value);
        }

        public IList<KeyValuePair<string, object>> GetAll()
        {
            return this._data;
        }
    }
}
