using FuncScript.Core;
using FuncScript.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FuncScript
{
    public class DefaultFsDataProvider : KeyValueCollection
    {
        static Dictionary<string, IFsFunction> s_funcByName = new Dictionary<string, IFsFunction>();
        static DefaultFsDataProvider()
        {
            LoadFromAssembly(Assembly.GetExecutingAssembly()); //always load builtin functions. May be we don't need this
        }

        public override bool Equals(object? obj)
        {
            if (!(obj is KeyValueCollection kvc))
                return false;
            return this.IsEqualTo(kvc);
        }

        public override int GetHashCode()
        {
            return this.GetKvcHashCode();
        }

        public KeyValueCollection ParentContext => null;
        public bool IsDefined(string key)
        {
            return s_funcByName.ContainsKey(key);
        }

        public IList<string> GetAllKeys()
        {
            return this._data.Keys.Concat(
                 s_funcByName.Keys.Where(k => !_data.ContainsKey(k))).ToList();
        }

        public static void LoadFromAssembly(Assembly a)
        {
            foreach (var t in a.GetTypes())
            {
                if (t.GetInterface(nameof(IFsFunction)) != null)
                {
                    if (t.GetConstructor(Type.EmptyTypes) != null) //load only functions with default constructor
                    {
                        var f = Activator.CreateInstance(t) as IFsFunction;
                        var lower = f.Symbol.ToLower();
                        if (!s_funcByName.TryAdd(lower, f))
                            throw new Exception($"{f.Symbol} already defined");
                        var aliace=t.GetCustomAttribute<FunctionAliasAttribute>();
                        if (aliace != null)
                        {
                            foreach (var al in aliace.Aliaces)
                            {
                                lower = al.ToLowerInvariant();
                                if (!s_funcByName.TryAdd(lower, f))
                                    throw new Exception($"{f.Symbol} already defined");
                            }

                        }
                    }
                }
            }
        }
        Dictionary<string, object> _data;
        public DefaultFsDataProvider()
        {
            _data = null;
        }
        public DefaultFsDataProvider(IList<KeyValuePair<string, object>> data)
        {
            _data = new Dictionary<string, object>();
            foreach (var k in data)
            {
                if (k.Value is Func<object>)
                    _data.Add(k.Key, k.Value);
                else
                    _data.Add(k.Key, Helpers.NormalizeDataType(k.Value));
            }
        }
        public object Get(string name)
        {
            if (_data != null)
            {
                if (_data.TryGetValue(name, out var v))
                {
                    if (v is Func<object>)
                    {
                        v = ((Func<object>)v)();
                        _data[name] = v;
                    }
                    return v;
                }
            }
            if (s_funcByName.TryGetValue(name, out var ret))
                return ret;
            return null;
        }
    }

    /// <summary>
    /// KeyValueCollection backed by KeyValueCollection
    /// </summary>
    public class KvcProvider:KeyValueCollection
    {
        public KeyValueCollection ParentContext => _parent;
        KeyValueCollection _kvc;
        KeyValueCollection _parent;
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
        public KvcProvider(KeyValueCollection kvc, KeyValueCollection parent)
        {
            _kvc = kvc;
            _parent = parent;
        }

        public object Get(string name)
        {
            if (_kvc.IsDefined(name))
                return _kvc.Get(name);
            if (_parent == null)
                return null;
            return _parent.Get(name);
        }
        public bool IsDefined(string key)
        {
            if (_kvc.IsDefined(key))
                return true;
            if (_parent != null)
                return _parent.IsDefined(key);
            return false;
        }

        public IList<string> GetAllKeys()
        {
            return _kvc.GetAllKeys();
        }
    }

}
