using funcscript.core;
using funcscript.error;
using funcscript.model;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.Json;
using System.Xml.XPath;
using funcscript.block;
using Newtonsoft.Json.Serialization;

namespace funcscript
{
    public static partial class FuncScript
    {
        static HashSet<Type> _useJson;
        static Newtonsoft.Json.JsonSerializerSettings _nsSetting;
        static FuncScript()
        {
            _nsSetting = new Newtonsoft.Json.JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver()
            };
            _useJson = new HashSet<Type>();
        }
        public static void NormalizeUsingJson<T>()
        {
            var t = typeof(T);
            if (!_useJson.Contains(t))
                _useJson.Add(t);
        }
        static object FromJToken(JToken p)
        {
            object val;
            switch (p.Type)
            {
                case JTokenType.None:
                    return null;
                case JTokenType.Object:
                    return FromJObject(p as JObject);
                case JTokenType.Array:
                    var jarr = (JArray)p;
                    object[] a = new object[jarr.Count];
                    for (int i = 0; i < a.Length; i++)
                        a[i] = FromJToken(jarr[i]);
                    return new ArrayFsList(a);
                case JTokenType.Constructor:
                    return null;
                case JTokenType.Property:
                    return null;
                case JTokenType.Comment:
                    return null;
                case JTokenType.Integer:
                    try
                    {
                        return (int)p;
                    }
                    catch (OverflowException)
                    {
                        return (long)p;
                    }
                case JTokenType.Float:
                    return (double)(float)p;
                case JTokenType.String:
                    return (string)p;
                case JTokenType.Boolean:
                    return (bool)p;
                case JTokenType.Null:
                    return null;
                case JTokenType.Undefined:
                    return null;
                case JTokenType.Date:
                    return (DateTime)p;
                case JTokenType.Raw:
                    return null;
                case JTokenType.Bytes:
                    return (byte[])p;
                case JTokenType.Guid:
                    return (Guid)p;
                case JTokenType.Uri:
                    return (string)p;
                case JTokenType.TimeSpan:
                    return null;
                default:
                    return null;
            }
        }
        static KeyValueCollection FromJObject( JObject jobj)
        {
            var pairs = new List<KeyValuePair<string, object>>();
            foreach (var p in jobj)
            {
                pairs.Add(new KeyValuePair<string, object>(p.Key, FromJToken(p.Value)));
            }
            return new SimpleKeyValueCollection(null,pairs.ToArray());

        }
        public static object FromJson(String json)
        {
            var t = Newtonsoft.Json.Linq.JToken.Parse(json);
            return FromJToken(t);
        }
        /// <summary>
        /// Converts a .net value into type that is compatible with FuncScript
        /// </summary>
        /// <param name="value">value to convert</param>
        /// <returns></returns>
        public static object NormalizeDataType(object value)
        {
            if (value == null)
                return null;

            if (value is byte[])
            {
                return value;
            }
            var t = value.GetType();


            if (value == null
                || value is bool || value is long || value is Guid || value is string  //simple dataa
                || value is DateTime
                || value is KeyValueCollection   //compound data
                || value is IFsFunction    //we treat function as a data. Function objects should not retain state
                || value is ByteArray
                || value is FsList
                || value is FsError
                )
            {
                return value; ;
            }
            if (value is decimal)
            {
                return (double)(decimal)value;
            }
            if (value is int || value is short || value is byte) //we use only int32 and int64
            {
                return Convert.ToInt32(value);
            }

            if (value is float || value is double) //we use only double floating number type
            {
                return Convert.ToDouble(value);
            }

            if (t.IsEnum)
            {
                return value.ToString();
            }
            if (value is Delegate @delegate)
            {
                return new DelegateFunction(@delegate);

            }
            if (value is JToken token)
            {
                return collect(token);
            }
            if (value is JsonElement)
            {
                return collect((JsonElement)value);
            }
            if (_useJson.Contains(value.GetType()))
            {
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(value, _nsSetting);
                var obj = FuncScript.Evaluate(json);
                return obj;
            }
            if (FsList.IsListType(t))
            {
                return new ArrayFsList(value);
            }
            
            return new ObjectKvc(value);
        }
        static object collect(JsonElement el)
        {
            return el.ValueKind switch
            {
                JsonValueKind.Array => new ArrayFsList(el.EnumerateArray().Select(x => collect(x)).ToArray()),
                JsonValueKind.String => el.GetString(),
                JsonValueKind.Object => new SimpleKeyValueCollection(null,el.EnumerateObject().Select(x =>
                                    new KeyValuePair<string, object>(x.Name, collect(x.Value))
                                    ).ToArray()),
                JsonValueKind.Number => el.GetDouble(),
                JsonValueKind.Null => null,
                JsonValueKind.False => false,
                JsonValueKind.True => true,
                JsonValueKind.Undefined => null,
                _ => null,
            };
        }
        static object collect(JToken obj)
        {
            if (obj == null)
                return null;
            if (obj is JValue)
            {
                var v = obj as JValue;
                return NormalizeDataType(v.Value);
            }
            if (obj is JProperty)
            {
                var v = obj as JProperty;
                return new KeyValuePair<string, object>(v.Name, collect(v.Value));
            }
            if (obj is JObject)
            {
                var o = obj as JObject;
                var arr = obj.Select(x => collect(x)).ToArray();
                var kv = true;
                foreach (var k in arr)
                {
                    if (!(k is KeyValuePair<string, object>))
                    {
                        kv = false;
                        break;
                    }
                }
                if (kv)
                    return new SimpleKeyValueCollection(null,arr.Select(x => (KeyValuePair<string, object>)x).ToArray());
                return arr;
            }
            if (obj is JArray)
            {
                var a = obj as JArray;
                var arr = obj.Select(x => collect(x)).ToArray();
                return arr;
            }
            throw new InvalidOperationException($"Unsupported json object type {obj.GetType()}");
        }

        const string TAB = "  ";
        private const int BREAK_LINE_THRUSHOLD = 80;
        public static bool IsAttomicType(object val)
        {
            return val == null ||
                val is bool ||
                    val is int ||
                    val is long ||
                    val is double ||
                    val is string;
        }

        public static FSDataType GetFsDataType(object value)
        {
            if (value == null)
                return FSDataType.Null;
            if (value is bool)
                return FSDataType.Boolean;
            if (value is int)
                return FSDataType.Integer;
            if (value is double)
                return FSDataType.Float;
            if (value is long)
                return FSDataType.BigInteger;
            if (value is Guid)
                return FSDataType.Guid;
            if (value is string)
                return FSDataType.String;
            if (value is byte[])
                return FSDataType.ByteArray;
            if (value is FsList)
                return FSDataType.List;
            if (value is KeyValueCollection)
                return FSDataType.KeyValueCollection;
            if (value is IFsFunction)
                return FSDataType.Function;
            if (value is FsError)
                return FSDataType.Error;
            throw new error.UnsupportedUnderlyingType($"Unsupported .net type {value.GetType()}");
        }
        public static bool IsNumeric(object val)
        {
            return val is int || val is double || val is long;
        }
        internal static bool ConvertToCommonNumericType(object v1, object v2, out object v1out, out object v2out)
        {
            if (v1.GetType() == v2.GetType())
            {
                v1out = v1;
                v2out = v2;
                return true;
            }
            if (v1 is int)
            {
                if (v2 is long)
                {
                    v1out = Convert.ToInt64(v1);
                    v2out = v2;
                    return true;
                }
                if (v2 is double)
                {
                    v1out = Convert.ToDouble(v1);
                    v2out = v2;
                    return true;
                }
                else
                {
                    v1out = null;
                    v2out = null;
                    return false;
                }
            }
            else if (v1 is long)
            {
                if (v2 is int)
                {
                    v1out = v1;
                    v2out = Convert.ToInt64(v2);
                    return true;
                }
                if (v2 is double)
                {
                    v1out = Convert.ToDouble(v1);
                    v2out = v2;
                    return true;
                }
                else
                {
                    v1out = null;
                    v2out = null;
                    return false;
                }
            }
            else if (v1 is double)
            {
                if (v2 is int)
                {
                    v1out = v1;
                    v2out = Convert.ToDouble(v2);
                    return true;
                }
                if (v2 is long)
                {
                    v1out = v1;
                    v2out = Convert.ToDouble(v2);
                    return true;
                }
                else
                {
                    v1out = null;
                    v2out = null;
                    return false;
                }
            }
            else
            {
                v1out = null;
                v2out = null;
                return false;
            }
        }

        public static object Evaluate(string expression)
        {
            return Evaluate(expression, new DefaultFsDataProvider(), null, ParseMode.Standard);
        }

        public static T ConvertFromFSObject<T>(object obj) where T : class
        {
            if (obj is KeyValueCollection)
            {
                return (T)((KeyValueCollection)obj).ConvertTo(typeof(T));
            }
            if (obj is null)
                return null;
            return (T)obj;
        }
        public static object EvaluateSpaceSeparatedList(string expression)
        {
            return Evaluate(expression, new DefaultFsDataProvider(), null, ParseMode.SpaceSeparatedList);
        }
        public static object EvaluateWithVars(string expression, object vars)
        {
            return Evaluate(expression, new DefaultFsDataProvider(), vars, ParseMode.Standard);
        }
        public static object Evaluate(KeyValueCollection providers, string expression)
        {
            return Evaluate(expression, providers, null, ParseMode.Standard);
        }
        public enum ParseMode
        {
            Standard,
            SpaceSeparatedList,
            FsTemplate
        }
        public static object Evaluate(string expression, KeyValueCollection provider, object vars, ParseMode mode)
        {
            if (vars != null)
            {
                provider = new KvcProvider(new ObjectKvc(vars), provider);
            }

            var context =
                new FuncScriptParser.ParseContext(provider, expression, new List<FuncScriptParser.SyntaxErrorData>());
            ExpressionBlock exp;
            switch (mode)
            {
                case ParseMode.Standard:
                    exp = core.FuncScriptParser.Parse(context).Expression;
                    break;
                case ParseMode.SpaceSeparatedList:
                    return core.FuncScriptParser.ParseSpaceSepratedList(context);
                case ParseMode.FsTemplate:
                    exp = core.FuncScriptParser.ParseFsTemplate(context).Expression;
                    break;
                default:    
                    exp = null;
                    break;
            }

            if (exp == null)
                throw new error.SyntaxError(context.Expression, context.SyntaxErrors);
            return Evaluate(exp, expression, provider, vars);
        }
        public static object Evaluate(ExpressionBlock exp, string expression, KeyValueCollection provider, object vars)
        {
            try
            {
                var ret=exp.Evaluate();
                return ret;
            }
            catch (EvaluationException ex)
            {
                String msg;
                if (ex.Len + ex.Pos <= expression.Length && ex.Len > 0)
                    msg = $"Evaluation error at '{expression.Substring(ex.Pos, ex.Len)}'";
                else
                    msg = $"Evaluation error. Location information invalid"; ;
                throw new EvaluationException(msg, ex.Pos, ex.Len, ex.InnerException);
            }
        }

        public static bool ValueEqual(object val1, object val2)
        {
            var t1 = GetFsDataType(val1);
            var t2 = GetFsDataType(val2);
            if (t1 != t2)
                return false;
            if (val1 == null && val2 == null)
                return true;
            if (val1 == null ^ val2 == null)
                return false;
            
            if (val1 is KeyValueCollection kv1 && val2 is KeyValueCollection kv2)
            {
                foreach (var key in kv1.GetAllKeys())
                {
                    if (!kv2.IsDefined(key.ToLowerInvariant()))
                        return false;
                    if (!ValueEqual(kv1.Get(key), kv2.Get(key)))
                        return false;
                }

                return true;
            }
            if (val1 is FsList lst1 && val2 is FsList lst2)
            {
                if (lst1.Length != lst2.Length)
                    return false;
                var e1 = lst1.GetEnumerator();
                var e2 = lst2.GetEnumerator();
                while (e1.MoveNext())
                {
                    e2.MoveNext();
                    if (!ValueEqual(e1.Current, e2.Current))
                        return false;
                }
                return true;
            }
            return val1.Equals(val2);
        }
    }
}
