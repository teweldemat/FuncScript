﻿using System.ComponentModel.Design;
using funcscript.core;
using funcscript.model;
using System.Security.Cryptography;
using System.Text;

namespace funcscript.block
{
    
    public class KvcExpression : ExpressionBlock
    {
         class ExpressionKvc : KeyValueCollection
        {
            private IFsDataProvider _provider;
            private KvcExpression _parent;
            public ExpressionKvc(IFsDataProvider provider, KvcExpression parent)
            {
                this._provider = provider;
                _parent = parent;
            }

            public override object Get(string key)
            {
                return _provider.GetData(key.ToLower());

            }

            public override bool ContainsKey(string key)
            {
                return _parent.index.ContainsKey(key.ToLower());
            }

            public override IList<KeyValuePair<string, object>> GetAll()
            {
                return _parent.KeyValues.Select(x => new KeyValuePair<String, Object>(x.Key, _provider.GetData(x.KeyLower))).ToArray();
            }
        }
       
        class KvcProvider : IFsDataProvider
        {
            IFsDataProvider ParentProvider;
            Dictionary<string, object> vals = new Dictionary<string, object>();
            KvcExpression Parent;
            public KvcProvider(KvcExpression Parent, IFsDataProvider ParentProvider)
            {
                this.Parent = Parent;
                this.ParentProvider = ParentProvider;
            }
            String _evaluating = null;
            public object GetData(string name)
            {
                if (vals.TryGetValue(name, out var val))
                    return val;
                if (_evaluating == null || name != _evaluating)
                {
                    if (Parent.index.TryGetValue(name, out var exp) && exp.ValueExpression != null)
                    {
                        _evaluating = name;
                        var v = exp.ValueExpression.Evaluate(this);
                        _evaluating = null;
                        vals[name] = v;
                        return v;
                    }
                }
                return ParentProvider.GetData(name);
            }
        }
        public class KeyValueExpression
        {
            public String Key;
            public String KeyLower;
            public ExpressionBlock ValueExpression;
        }

        public class ConnectionExpression
        {
            public ExpressionBlock Source;
            public ExpressionBlock Sink;
            public ExpressionBlock Catch;
        }
        IList<KeyValueExpression> _keyValues;
        private IList<ConnectionExpression> _connections;
        Dictionary<string, KeyValueExpression> index;
        KeyValueExpression singleReturn = null;
        public String SetKeyValues(IList<KeyValueExpression> kv,IList<ConnectionExpression> conns)
        {
            _keyValues = kv;
            _connections = conns;   
            if (_keyValues == null)
                index = null;
            {
                index = new Dictionary<string, KeyValueExpression>();
                singleReturn = null;
                foreach (var k in _keyValues)
                {
                    if (k.Key == null)
                    {
                        if (this.singleReturn != null)
                            return "Ambigues return expressions";
                        this.singleReturn = k;
                    }
                    else
                    {
                        k.KeyLower = k.Key.ToLower();
                        if (this.index.ContainsKey(k.KeyLower))
                            return $"Key {k.KeyLower} is duplicated";
                        this.index.Add(k.KeyLower, k);
                    }

                }
            }
            return null;
        }
        public IList<KeyValueExpression> KeyValues => _keyValues;
        public override object Evaluate(IFsDataProvider provider)
        {
            var p = new KvcProvider(this, provider);
            foreach (var connection in this._connections)
            {
                var source = connection.Source.Evaluate(p);
                var sink = connection.Sink.Evaluate(p);
                if (sink is ValueSinkDelegate valSink)
                {
                    valSink(source);
                }
                else if (source is SignalSourceDelegate sigSource)
                {
                    SignalListenerDelegate sinkListner;
                    if (sink is SignalListenerDelegate l)
                        sinkListner = l;
                    else
                    {
                        sinkListner = () =>
                        {
                            var x = FuncScript.Dref(sink);
                            if (x is SignalListenerDelegate s)
                                s();
                        };
                    }
                    
                    sigSource(sink,connection.Catch?.Evaluate(p));
                }
                else 
                    throw new error.EvaluationTimeException("Invalid connection");
                
            }
            if (this.singleReturn == null)
            {
                return new ExpressionKvc(p, this);
                //var kvc = KeyValues.Select(x => new KeyValuePair<String, Object>(x.Key, p.GetData(x.KeyLower))).ToArray();
                //return new SimpleKeyValueCollection(kvc);
            }
            else
            {
                return this.singleReturn.ValueExpression.Evaluate(p);
            }

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
        public override string AsExpString(IFsDataProvider provider)
        {
            var sb = new StringBuilder();
            sb.Append("{\n");
            foreach(var kv in this.KeyValues)
            {
                sb.Append($"\t\n{kv.Key}: {kv.ValueExpression.AsExpString(provider)},");
            }
            if(this.singleReturn!=null)
            {
                sb.Append($"return {this.singleReturn.ValueExpression.AsExpString(provider)}");
            }
            sb.Append("}");
            return sb.ToString();
        }
    }
}
