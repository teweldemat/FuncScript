using System.ComponentModel.Design;
using System.Data;
using FuncScript.Core;
using FuncScript.Model;
using System.Security.Cryptography;
using System.Text;
using FuncScript.Error;

namespace FuncScript.Block
{
    public class KvcExpression : ExpressionBlock, KeyValueCollection
    {
        public class KeyValueExpression
        {
            public String Key;
            public String KeyLower;
            public ExpressionBlock ValueExpression;

            public KeyValueExpression Clone()
            {
                return new KeyValueExpression
                {
                    Key = this.Key,
                    KeyLower = this.KeyLower,
                    ValueExpression = ValueExpression.CloneExpression()
                };
            }
        }
        public IList<KeyValueExpression> _keyValues;
        public ExpressionBlock SingleReturn = null;
        private Dictionary<string, KeyValueExpression> index;
        public override bool Equals(object obj)
        {
            if (!(obj is KeyValueCollection kvc))
                return false;
            return this.IsEqualTo(kvc);
        }

        public override int GetHashCode()
        {
            return this.GetKvcHashCode();
        }
        public string SetKeyValues(IList<KeyValueExpression> kv, ExpressionBlock retExpression)
        {
            _keyValues = kv;

            this.SingleReturn = retExpression;
            if (retExpression != null)
                retExpression.SetReferenceProvider(this);
            if (_keyValues == null)
                index = null;
            else
            {
                index = new Dictionary<string, KeyValueExpression>();
                foreach (var k in _keyValues)
                {
                    k.ValueExpression.SetReferenceProvider(this);
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
            if (SingleReturn != null)
                return SingleReturn.Evaluate();
            return this;
        }
        public override void SetReferenceProvider(KeyValueCollection provider)
        {
            this._context = provider;
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

            if (this.SingleReturn != null)
            {
                sb.Append($"return {this.SingleReturn.AsExpString()}");
            }

            sb.Append("}");
            return sb.ToString();
        }

        public object Get(string name)
        {
            if (this.index.TryGetValue(name, out var e))
                return e.ValueExpression.Evaluate();
            if (ParentContext != null)
                return ParentContext.Get(name);
            return null;
        }

        private KeyValueCollection _context;
        public KeyValueCollection ParentContext => _context;
        public bool IsDefined(string key)
        {
            return this.index.ContainsKey(key);
        }

        public IList<string> GetAllKeys()
        {
            return this.KeyValues
                .Select(kv => kv.Key).ToList();
        }

        public IList<KeyValuePair<string, object>> GetAll()
        {
            return this.KeyValues
                .Select(kv => KeyValuePair.Create(kv.Key, kv.ValueExpression.Evaluate()))
                .ToList();
        }

        public override ExpressionBlock CloneExpression()
        {
            var ret = new KvcExpression();
            ret.SetKeyValues(this.KeyValues.Select(kv =>
                kv.Clone()).ToArray(), this.SingleReturn == null ? null : this.SingleReturn.CloneExpression());
            ret.CodePos = this.CodePos;
            ret.CodeLength = this.CodeLength;

            return ret;
        }
    }
}
