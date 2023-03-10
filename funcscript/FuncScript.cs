using funcscript.core;
using funcscript.error;
using funcscript.model;
using Newtonsoft.Json.Linq;
using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Text;

namespace funcscript
{
    public static class FuncScript
    {
     
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
                    for (int i = 0; i<a.Length; i++)
                        a[i] = FromJToken(jarr[i]);
                    return new FsList(a);
                case JTokenType.Constructor:
                    return null;
                case JTokenType.Property:
                    return null;
                case JTokenType.Comment:
                    return null;
                case JTokenType.Integer:
                    return (int)p;
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
                    return null; ;
            }
        }
        static KeyValueCollection FromJObject(JObject jobj)
        {
            var pairs = new List<KeyValuePair<string, object>>();
            foreach(var p in jobj)
            {
                pairs.Add(new KeyValuePair<string, object>(p.Key, FromJToken(p.Value)));
            }
            return new SimpleKeyValueCollection(pairs.ToArray());

        }
        public static object FromJson(String json)
        {
            var t=Newtonsoft.Json.Linq.JToken.Parse(json);
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
            
            if(value is byte[])
            {
                return value;
            }
            var t = value.GetType();

            if (FsList.IsListTyp(t))
            {
                return new FsList(value);
            }
            if (value == null
                || value is bool || value is long || value is Guid || value is string  //simple dataa
                || value is DateTime
                || value is KeyValueCollection   //compound data
                || value is IFsFunction    //we treat function as a data. Function objects should not retain state
                || value is ByteArray
                || value is FsList
                )
            {
                return value; ;
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
            if(value is Delegate)
            {
                return new DelegateFunction((Delegate)value);
                
            }
            return new ObjectKvc(value);
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
                    return new SimpleKeyValueCollection(arr.Select(x => (KeyValuePair<string, object>)x).ToArray());
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

        /// <summary>
        /// Formats a value into string
        /// </summary>
        /// <param name="sb">A string builder object</param>
        /// <param name="val">Value to format</param>
        /// <param name="format">Optional formatting parameter </param>
        /// <param name="asFuncScriptLiteral">Format as FuncScript literal</param>
        /// <param name="asJsonLiteral">Format as JSON literal</param>
        public static void Format(StringBuilder sb, object val, string format=null,
            bool asFuncScriptLiteral=false,
            bool asJsonLiteral=false)
        {
            Format("", sb, val, format,asFuncScriptLiteral,asJsonLiteral,true);
        }
        static String TestFormat(object val, string format = null,
            bool asFuncScriptLiteral = false,
            bool asJsonLiteral = false)
        {
            var sb = new StringBuilder();
            Format("", sb, val, format, asFuncScriptLiteral, asJsonLiteral, false);
            return sb.ToString();
        }
        static void Format(String indent, StringBuilder sb, object val,
            string format,
            bool asFuncScriptLiteral,
            bool asJsonLiteral,bool adaptiveLineBreak)
        {
            if (val == null)
            {
                sb.Append("null");
                return;
            }
            if(val is ByteArray)
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
                var list = (FsList)val;
                bool useLineBreak = false;
                if(adaptiveLineBreak)
                {
                    var test = TestFormat(val, format, asFuncScriptLiteral, asJsonLiteral);
                    useLineBreak = test.Length > BREAK_LINE_THRUSHOLD;
                }
                sb.Append($"[");
                if (list.Data.Length > 0)
                {
                    if(useLineBreak)
                        sb.Append($"\n{indent}{TAB}");
                    else
                        sb.Append($" ");
                    Format($"{indent}{TAB}", sb, list.Data[0], format, asFuncScriptLiteral, asJsonLiteral,adaptiveLineBreak);
                    for (int i = 1; i < list.Data.Length; i++)
                    {
                        if(useLineBreak)
                            sb.Append($",\n{indent}{TAB}");
                        else
                            sb.Append($", ");
                        Format($"{indent}{TAB}", sb, list.Data[i], format, asFuncScriptLiteral, asJsonLiteral, adaptiveLineBreak);
                    }
                }
                if(useLineBreak)
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
                    var test = TestFormat(val, format, asFuncScriptLiteral, asJsonLiteral);
                    useLineBreak = test.Length > BREAK_LINE_THRUSHOLD;
                }

                var kv = (KeyValueCollection)val;
                if(useLineBreak)
                    sb.Append($"{{\n");
                else
                    sb.Append("{ ");
                var pairs = kv.GetAll();
                if (pairs.Count > 0)
                {
                    var pair = pairs[0];
                    if(useLineBreak)
                        sb.Append($"{indent}{TAB}\"{pair.Key}\":");
                    else
                        sb.Append($"\"{pair.Key}\":");
                    Format($"{indent}{TAB}", sb, pair.Value, format, asFuncScriptLiteral, asJsonLiteral,adaptiveLineBreak);
                    for (int i = 1; i < pairs.Count; i++)
                    {
                        if (useLineBreak)
                            sb.Append(",\n");
                        else
                            sb.Append(", ");

                        pair = pairs[i];
                        if(useLineBreak)
                            sb.Append($"{indent}{TAB}\"{pair.Key}\":");
                        else
                            sb.Append($"\"{pair.Key}\":");
                        Format($"{indent}{TAB}", sb, pair.Value, format, asFuncScriptLiteral, asJsonLiteral, adaptiveLineBreak);
                    }
                }
                if(useLineBreak)
                    sb.Append($"\n{indent}}}");
                else
                    sb.Append("}");
                return;
            }
            if(val is bool)
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
            if(val is double)
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
            if (val is string)
            {
                if (asJsonLiteral || asFuncScriptLiteral)

                {
                    sb.Append("\"");
                    foreach (var ch in (string)val)
                    { 
                        switch(ch)
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
                                if(asFuncScriptLiteral)
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
                    sb.Append("\"");
                }
                else
                    sb.Append((string)val);
                return;
            }
            if (asJsonLiteral || asFuncScriptLiteral)
                sb.Append("\"");
            sb.Append(val.ToString());
            if (asJsonLiteral || asFuncScriptLiteral)
                sb.Append("\"");
        }

        /// <summary>
        /// Gets the data type of a value as FSDataType
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="error.UnsupportedUnderlyingType"></exception>
        public static FSDataType GetFsDataType(object value)
        {
            if (value == null)
                return FSDataType.Null;
            if (value is bool)
                return FSDataType.Boolean;
            if (value is int)
                return FSDataType.Integer;
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
            if(value is IFsFunction)
                return FSDataType.Function;
            throw new error.UnsupportedUnderlyingType($"Unsupported .net type {value.GetType()}");
        }
        internal static bool IsNumeric(object val)
        {
            return val is int || val is double || val is long;
        }
        internal static bool ConvertToCommonNumbericType(object v1, object v2, out object v1out, out object v2out)
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
                    v1out = Convert.ToUInt64(v1);
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
                    v2out = Convert.ToUInt64(v2);
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
            return Evaluate(expression, new DefaultFsDataProvider(),null,ParseMode.Standard);
        }
        public static object EvaluateSpaceSeparatedList(string expression)
        {
            return Evaluate(expression, new DefaultFsDataProvider(), null, ParseMode.SpaceSeparatedList);
        }
        public static object EvaluateWithVars(string expression,object vars)
        {
            return Evaluate(expression, new DefaultFsDataProvider(), vars, ParseMode.Standard);
        }
        public static object Evaluate(string expression, IFsDataProvider providers)
        {
            return Evaluate(expression, providers, null,ParseMode.Standard);
        }
        public enum ParseMode
        {
            Standard,
            SpaceSeparatedList
        }
        public static object Evaluate(string expression, IFsDataProvider provider,object vars,ParseMode mode)
        {
            if(vars!=null)
            {
                provider = new KvcProvider(new ObjectKvc(vars), provider);
            }
            var serrors = new List<FuncScriptParser.SyntaxErrorData>();
            ExpressionBlock exp;
            switch (mode)
            {
                case ParseMode.Standard:
                    exp= core.FuncScriptParser.Parse(provider, expression, serrors);
                    break;
                case ParseMode.SpaceSeparatedList:
                    return core.FuncScriptParser.ParseSpaceSepratedList(provider, expression, serrors);
                default:
                    exp=null;
                    break;
            }
            
            if (exp == null)
                throw new error.SyntaxError(serrors);
            return Evaluate(exp, expression, provider, vars);
        }
        public static object Evaluate(ExpressionBlock exp, string expression,IFsDataProvider provider, object vars)
        {
            try
            {
                return exp.Evaluate(provider);
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


    }
}
