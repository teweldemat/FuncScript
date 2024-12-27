using System.ComponentModel.Design;
using System.Data;
using funcscript.core;
using funcscript.model;
using System.Security.Cryptography;
using System.Text;
using funcscript.error;

namespace funcscript.block
{
    public class KvcExpression : ExpressionBlock,KeyValueCollection
    {
        private class KvcExpressionProvider : IFsDataProvider
        {
            private readonly KvcExpression _parent;
            private readonly Dictionary<string, object> _valCache = new Dictionary<string, object>();
            public IFsDataProvider ParentProvider { get; }

            public bool IsDefined(string key)
            {
                return _parent.index.ContainsKey(key);
            }

            public KvcExpressionProvider(IFsDataProvider provider, KvcExpression parent)
            {
                this.ParentProvider = provider;
                _parent = parent;
            }
            String _evaluating = null;
            public object Get(string name)
            {
                if (_valCache.TryGetValue(name, out var val))
                    return val;
                if (_evaluating == null || name != _evaluating)
                {
                    if (_parent.index.TryGetValue(name, out var exp) && exp.ValueExpression != null)
                    {
                        _evaluating = name;
                        var v = exp.ValueExpression.Evaluate();
                        _evaluating = null;
                        _valCache[name] = v;
                        return v;
                    }
                }
                return ParentProvider.Get(name);
            }
        }

        public class KeyValueExpression
        {
            public String Key;
            public String KeyLower;
            public ExpressionBlock ValueExpression;
        }
        public IList<KeyValueExpression> _keyValues;
        public ExpressionBlock singleReturn = null;
        private Dictionary<string, KeyValueExpression> index;
        public String SetKeyValues(IList<KeyValueExpression> kv, ExpressionBlock retExpression)
        {
            _keyValues = kv;
            
            this.singleReturn = retExpression;
            if (retExpression != null)
                retExpression.Provider = this;
            if (_keyValues == null)
                index = null;
            else
            {
                index = new Dictionary<string, KeyValueExpression>();
                foreach (var k in _keyValues)
                {
                    k.ValueExpression.Provider = this;
                    k.KeyLower = k.Key.ToLower();
                    if (this.index.ContainsKey(k.KeyLower))
                        return $"Key {k.KeyLower} is duplicated";
                    this.index.Add(k.KeyLower, k);
                }
            }

            return null;
        }

        public IList<KeyValueExpression> KeyValues => _keyValues;

        public override object Evaluate()
        {
            return this;
            var evalProvider = new KvcExpressionProvider(Provider, this);

            var kvc = new SimpleKeyValueCollection(null, this._keyValues
                .Select(kv => KeyValuePair.Create<string, object>(kv.Key,
                    evalProvider.Get(kv.KeyLower))).ToArray());

            if (singleReturn != null)
            {
                return singleReturn.Evaluate();
            }
            return kvc;
        }

        public override IList<ExpressionBlock> GetChilds()
        {
            var ret = new List<ExpressionBlock>();
            ret.AddRange(this.KeyValues.Select(x => x.ValueExpression));
            return ret;
        }

        public override string ToString()
        {
            return "Key-values";
        }

        public override string AsExpString()
        {
            var sb = new StringBuilder();
            sb.Append("{\n");
            foreach (var kv in this.KeyValues)
            {
                sb.Append($"\t\n{kv.Key}: {kv.ValueExpression.AsExpString()},");
            }

            if (this.singleReturn != null)
            {
                sb.Append($"return {this.singleReturn.AsExpString()}");
            }

            sb.Append("}");
            return sb.ToString();
        }

        public object Get(string name)
        {
            if (this.index.TryGetValue(name, out var e))
                return e.ValueExpression.Evaluate();
            return null;
        }
        public IFsDataProvider ParentProvider => Provider;
        public bool IsDefined(string key)
        {
            return this.index.ContainsKey(key);
        }

        public IList<KeyValuePair<string, object>> GetAll()
        {
            return this.KeyValues
                .Select(kv => KeyValuePair.Create(kv.Key, kv.ValueExpression.Evaluate()))
                .ToList();
        }

    }
}