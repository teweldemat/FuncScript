using System.Collections;
using System.Text;
using FuncScript.Model;

namespace FuncScript;
public static partial class Helpers
{

    class ListCache : FsList
    {
        private readonly FsList _list;
        private int? _length;
        private readonly Dictionary<int, object> _valCache = new Dictionary<int, object>();

        public ListCache(FsList list)
        {
            _list = list;
        }

        public object this[int index]
        {
            get
            {
                if (_valCache.TryGetValue(index, out var val))
                    return val;

                var newVal = _list[index];
                _valCache.Add(index, newVal);
                return newVal;
            }
        }

        public int Length => _length ??= _list.Length;

        IEnumerator<object> FsList.GetEnumerator()
        {
            for (int i = 0; i < Length; i++)
                yield return this[i];
        }

        IEnumerator<object> IEnumerable<object>.GetEnumerator()
            => ((FsList)this).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => ((IEnumerable<object>)this).GetEnumerator();
    }
    public class KvCache : KeyValueCollection
    {
        private KeyValueCollection _kvc;
        private Dictionary<string, object> _valCache = new Dictionary<string, object>();
        public KvCache(KeyValueCollection kvc)
        {
            _kvc = kvc;
        }
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
        public object Get(string key)
        {
            var keyLower = key.ToLower();
            if (_valCache.TryGetValue(keyLower, out var val))
                return val;
            var newVal = _kvc.Get(keyLower);
            _valCache.Add(keyLower, newVal);
            return newVal;
        }

        public KeyValueCollection ParentContext => throw new NotImplementedException("ParentContext not available");
        public bool IsDefined(string key)
        {
            return _kvc.IsDefined(key);
        }

        public IList<string> GetAllKeys()
        {
            return _kvc.GetAllKeys();
        }

    }


    class ListKvcCache
    {
        Dictionary<object, object> cache = new Dictionary<object, object>();
        public object Get(object o)
        {
            if (cache.TryGetValue(o, out var c))
                return c;
            var ret = o;
            if (o is FsList lst)
                ret = new ListCache(lst);
            if (o is KeyValueCollection kvc)
                ret = new KvCache(kvc);
            cache.Add(o, ret);
            return ret;
        }
    }
    public static void Format(StringBuilder sb, object val, string format = null,
        bool asFuncScriptLiteral = false,
        bool asJsonLiteral = false)
    {

        var c = new ListKvcCache();
        Format("", sb, val, format, asFuncScriptLiteral, asJsonLiteral, true, c.Get);
    }
        
    public static string FormatToJson(object val)
    {
        var sb = new StringBuilder();
        Format(sb, val, asJsonLiteral: true);
        return sb.ToString();
    }

    static string TestFormat(object val, Func<object, object> cacheObject, string format = null,
        bool asFuncScriptLiteral = false,
        bool asJsonLiteral = false)
    {
        var sb = new StringBuilder();
        Format("", sb, val, format, asFuncScriptLiteral, asJsonLiteral, false, cacheObject);
        return sb.ToString();
    }

    static void Format(string indent, StringBuilder sb, object val,
        string format,
        bool asFuncScriptLiteral,
        bool asJsonLiteral, bool adaptiveLineBreak,
        Func<object, object> objectCache)
    {
        if (val is FsError error)
        {
            sb.Append($"Error: {error.ErrorMessage}");
            sb.Append($"  type: {error.ErrorType}");
            if (error.ErrorData != null)
                sb.Append($"\n{error.ErrorData}");
            return;
        }

        if (val == null)
        {
            sb.Append("null");
            return;
        }

        if (val is ByteArray)
        {
            if (asFuncScriptLiteral || asFuncScriptLiteral)
                sb.Append("");
            sb.Append(Convert.ToBase64String(((ByteArray)val).Bytes));
            if (asFuncScriptLiteral || asFuncScriptLiteral)
                sb.Append("");
            return;
        }

        if (val is FsList)
        {
            var list = (FsList)objectCache(val);
            bool useLineBreak = false;
            if (adaptiveLineBreak)
            {
                var test = TestFormat(val, objectCache, format, asFuncScriptLiteral, asJsonLiteral);
                useLineBreak = test.Length > BREAK_LINE_THRESHOLD;
            }

            sb.Append($"[");
            if (list.Length > 0)
                if (list.Length > 0)
                {
                    if (useLineBreak)
                        sb.Append($"\n{indent}{TAB}");
                    else
                        sb.Append($" ");
                    Format($"{indent}{TAB}", sb, list[0], format, asFuncScriptLiteral, asJsonLiteral,
                        adaptiveLineBreak, objectCache);
                    for (int i = 1; i < list.Length; i++)
                    {
                        if (useLineBreak)
                            sb.Append($",\n{indent}{TAB}");
                        else
                            sb.Append($", ");
                        Format($"{indent}{TAB}", sb, list[i], format, asFuncScriptLiteral, asJsonLiteral, adaptiveLineBreak, objectCache);
                    }
                }
            if (useLineBreak)
                sb.Append($"\n{indent}]");
            else
                sb.Append($" ]");
            return;
        }
        if (val is KeyValueCollection)
        {
            bool useLineBreak = false;
            if (adaptiveLineBreak)
            {
                var test = TestFormat(val, objectCache, format, asFuncScriptLiteral, asJsonLiteral);
                useLineBreak = test.Length > BREAK_LINE_THRESHOLD;
            }

            var kv = (KeyValueCollection)objectCache(val);
            if (useLineBreak)
                sb.Append($"{{\n");
            else
                sb.Append("{ ");
            var pair_keys = kv.GetAllKeys();
            if (pair_keys.Count > 0)
            {
                var pair_key = pair_keys[0];
                var pair_val = kv.Get(pair_key);
                if (useLineBreak)
                    sb.Append($"{indent}{TAB}\"{pair_key}\":");
                else
                    sb.Append($"\"{pair_key}\":");
                Format($"{indent}{TAB}", sb, pair_val, format, asFuncScriptLiteral, asJsonLiteral, adaptiveLineBreak, objectCache);
                for (int i = 1; i < pair_keys.Count; i++)
                {
                    if (useLineBreak)
                        sb.Append(",\n");
                    else
                        sb.Append(", ");

                    pair_key = pair_keys[i];
                    pair_val = kv.Get(pair_key);
                    if (useLineBreak)
                        sb.Append($"{indent}{TAB}\"{pair_key}\":");
                    else
                        sb.Append($"\"{pair_key}\":");
                    Format($"{indent}{TAB}", sb, pair_val, format, asFuncScriptLiteral, asJsonLiteral, adaptiveLineBreak, objectCache);
                }
            }
            if (useLineBreak)
                sb.Append($"\n{indent}}}");
            else
                sb.Append("}");
            return;
        }
        if (val is bool)
        {
            sb.Append((bool)val ? "true" : "false");
            return;
        }
        if (val is int)
        {
            if (format == null)
                sb.Append(val.ToString());
            else
                sb.Append(((int)val).ToString(format));
            return;
        }
        if (val is long)
        {
            if (asJsonLiteral)
                sb.Append("\"");
            if (format == null)
                sb.Append(val.ToString());
            else
                sb.Append(((long)val).ToString(format));
            if (asJsonLiteral)
                sb.Append("\"");
            else if (asFuncScriptLiteral)
                sb.Append("L");
            return;
        }
        if (val is double)
        {
            if (format == null)
                sb.Append(val.ToString());
            else
                sb.Append(((double)val).ToString(format));
            return;
        }
        if (val is DateTime)
        {
            if (asJsonLiteral || asFuncScriptLiteral)
                sb.Append("\"");
            if (format == null)
                sb.Append(((DateTime)val).ToString("yyy-MM-dd HH:mm:ss"));
            else
                sb.Append(((DateTime)val).ToString(format));
            if (asJsonLiteral || asFuncScriptLiteral)
                sb.Append("\"");
            return;
        }
        if (val is Guid)
        {
            if (asJsonLiteral || asFuncScriptLiteral)
                sb.Append("\"");
            if (format == null)
                sb.Append(val.ToString());
            else
                sb.Append(((Guid)val).ToString(format));
            if (asJsonLiteral || asFuncScriptLiteral)
                sb.Append("\"");
            return;
        }
        if (val is double)
        {
            if (format == null)
                sb.Append(val.ToString());
            else
                sb.Append(((double)val).ToString(format));
            return;
        }
        if (val is string valStr)
        {
            if (asJsonLiteral || asFuncScriptLiteral)

            {
                sb.Append("\"");
                foreach (var ch in valStr)
                {
                    if (char.IsControl(ch)) // check if it's a control character
                    {
                        sb.Append("\\u" + ((int)ch).ToString("x4")); // append it in \uxxxx form
                    }
                    else
                    {
                        switch (ch)
                        {
                            case '\n':
                                sb.Append(@"\n");
                                break;
                            case '\r':
                                sb.Append(@"\r");
                                break;
                            case '\t':
                                sb.Append(@"\t");
                                break;
                            case '"':
                                sb.Append(@"\""");
                                break;
                            case '{':
                                if (asFuncScriptLiteral)
                                    sb.Append(@"\{");
                                else
                                    sb.Append(@"{");
                                break;
                            case '\\':
                                sb.Append(@"\\");
                                break;
                            default:
                                sb.Append(ch);
                                break;
                        }
                    }
                }
                sb.Append("\"");
            }
            else
                sb.Append(valStr);
            return;
        }
        if (asJsonLiteral || asFuncScriptLiteral)
            sb.Append("\"");
        sb.Append(val.ToString().Replace("\"", "\\\""));
        if (asJsonLiteral || asFuncScriptLiteral)
            sb.Append("\"");
    }
}
