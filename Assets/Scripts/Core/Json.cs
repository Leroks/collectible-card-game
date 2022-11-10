using System;
using System.Collections.Generic;

namespace StormWarfare.Core
{
    class JsonParser
    {
        int pos;
        string raw;

        public JsonParser(string raw)
        {
            this.raw = raw;
        }

        void skipWhite()
        {
            while (pos < raw.Length)
            {
                char c = raw[pos++];
                if (c == ' ' || c == '\r' || c == '\n' || c == '\t')
                {
                    continue;
                }

                if (c == '/' && pos + 1 < raw.Length && raw[pos] == '*' && raw[pos + 1] == '*')
                {
                    int end = raw.IndexOf("**/", pos + 2);
                    if (end == -1)
                    {
                        throw new Exception("Unclosed comment");
                    }
                    pos = end + 3;
                    continue;
                }
                pos--;
                break;
            }
        }

        internal object parseInternal()
        {
            while (pos < raw.Length)
            {
                skipWhite();
                char c = raw[pos++];
                /*if (c == ' ' || c == '\r' || c == '\n' || c == '\t')
                {
                    continue;
                }*/
                switch (c)
                {
                    case '{':
                        Dynamic Object = new Dynamic();
                        String field = null;
                        while (pos < raw.Length)
                        {
                            skipWhite();
                            c = raw[pos++];
                            /*if (c == ' ' || c == '\r' || c == '\n' || c == '\t')
                            {
                                continue;
                            }*/
                            switch (c)
                            {
                                case '}':
                                    return Object;

                                case '"':
                                    field = parseString();
                                    break;

                                case ':':
                                    if (field != null)
                                    {
                                        Object[field] = parseInternal();
                                        field = null;
                                    }
                                    break;

                                case ',':
                                    break;

                                default:
                                    return null;
                            }
                        }
                        return Object;

                    case '[':
                        List<object> list = new List<object>();
                        while (pos < raw.Length)
                        {
                            skipWhite();
                            c = raw[pos++];
                            /*if (c == ' ' || c == '\r' || c == '\n' || c == '\t')
                            {
                                continue;
                            }*/

                            switch (c)
                            {
                                case ']':
                                    return list;
                                case ',':
                                    break;
                                default:
                                    pos--;
                                    list.Add(parseInternal());
                                    break;
                            }

                        }
                        return list;

                    case 't':
                        if (test("rue"))
                        {
                            pos += 3;
                            return true;
                        }
                        break;

                    case 'f':
                        if (test("alse"))
                        {
                            pos += 4;
                            return false;
                        }
                        break;

                    case 'n':
                        if (test("ull"))
                        {
                            pos += 3;
                            return null;
                        }
                        break;

                    case '"':
                        return parseString();

                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                    case '-':
                        pos--;
                        return parseNumber();

                    default:
                        throw new Exception("Invalid json at " + pos + " " + (pos < raw.Length ? raw.Substring(pos, (int)MathF.Min(64, raw.Length - pos)) : "EOF"));
                }
            }
            return null;
        }

        private bool test(String check)
        {
            int len = raw.Length;
            int p = pos;
            for (int i = 0; i < check.Length; i++)
            {
                if (p >= len || raw[p] != check[i])
                {
                    return false;
                }
                p++;
            }
            return true;
        }

        private string parseString()
        {
            string buf = "";
            while (pos < raw.Length)
            {
                char c = raw[pos++];
                if (c == '"')
                {
                    break;
                }
                switch (c)
                {
                    case '\\':
                        if (pos < raw.Length)
                        {
                            c = raw[pos++];
                            switch (c)
                            {
                                case 'r':
                                    buf += '\r';
                                    break;
                                case 'n':
                                    buf += '\n';
                                    break;
                                case 't':
                                    buf += '\t';
                                    break;
                                case 'b':
                                    buf += (char)8;
                                    break;
                                case 'f':
                                    buf += (char)12;
                                    break;
                                case '/':
                                case '\\':
                                case '"':
                                    buf += c;
                                    break;
                                case 'u':
                                    char code = (char)Convert.ToInt32(raw.Substring(pos, 4), 16);
                                    buf += code;
                                    pos += 4;
                                    break;

                            }
                        }
                        else
                        {
                            return buf;
                        }
                        break;

                    default:
                        buf += c;
                        break;
                }
            }
            return buf;
        }

        private object parseNumber()
        {
            int start = pos;
            bool isHex = false;
            bool isFloat = false;
            while (pos < raw.Length)
            {
                char c = raw[pos++];
                if (c == '.') isFloat = true;
                else if (c == 'x' && !isHex)
                {
                    isHex = true;
                    start = pos;
                    continue;
                }
                if (!((c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F') || (c >= '0' && c <= '9') || c == '.' || c == 'E' || c == 'e' || c == '-' || c == '+')) break;
            }
            pos--;
            double d = 0;
            string sub = raw.Substring(start, pos - start);
            if (isHex)
            {
                int hex;
#if __WEB__
                if(int.TryParse(sub, out hex, 16))
#else
                if (int.TryParse(sub, System.Globalization.NumberStyles.HexNumber, null, out hex))
#endif
                    return hex;
            }
            else if (!isFloat)
            {
                if (int.TryParse(sub, out int i)) return i;
                else if (long.TryParse(sub, out long l)) return l;
            }
            else if (double.TryParse(sub, out d))
            {
                //int i= (int)d;
                //if (i == d) return i;
                //long l = (long)d;
                //if (l == d) return l;

                //cok buyuk bir deger gelirse float'a cast edip devam edelim
                return (float)d;
            }

            throw new Exception("Invalid number at " + start + " -> " + sub);
        }
    }

    public class Json
    {

        public static object parse(string str)
        {
            if (str == null || str.Length == 0) return null;
            try
            {
                return new JsonParser(str).parseInternal();
            }
            catch (Exception exception)
            {
                Console.Write(exception);
            }
            return null;
        }

        public static T parse<T>(string str) where T : class
        {
            var temp = parse(str);
            return temp as T;
        }

        public static string stringify(object obj)
        {
            try
            {
                return internalStringify(obj);
            }
            catch (Exception e)
            {
            }
            return "";
        }

        private static string internalStringify(object raw)
        {
            if (raw == null)
            {
                return "null";
            }

            if (raw is IDictionary<string, object>)
            {
                string ret = "{";
                var map = (IDictionary<string, object>)raw;
                int count = 0;
                foreach (var key in map.Keys)
                {
                    if (count++ > 0)
                    {
                        ret += ",";
                    }
                    ret += quote(key) + ":" + internalStringify(map[key]);
                }
                return ret + "}";
            }
            else if (raw is IList<object>)
            {
                var list = (IList<object>)raw;
                string ret = "[";
                for (int i = 0; i < list.Count; i++)
                {
                    if (i > 0)
                    {
                        ret += ",";
                    }
                    ret += internalStringify(list[i]);
                }
                return ret + "]";
            }
            else if (raw is List<int>)
            {
                var list = (List<int>)raw;
                string ret = "[";
                for (int i = 0; i < list.Count; i++)
                {
                    if (i > 0)
                    {
                        ret += ",";
                    }
                    ret += internalStringify(list[i]);
                }
                return ret + "]";
            }
            else if (raw is string)
            {
                return quote(raw.ToString());
            }
            else if (raw is bool)
                return ((bool)raw) ? "true" : "false";
            else if (raw is ValueType)
            {
                return raw.ToString();
            }
            return "{}";
        }

        private static string quote(String str)
        {
            string ret = "\"";
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                switch (c)
                {
                    case '"':
                        ret += "\\\"";
                        break;
                    case '\\':
                        ret += "\\\\";
                        break;
                    case '\n':
                        ret += "\\n";
                        break;
                    case '\r':
                        ret += "\\r";
                        break;
                    case '\t':
                        ret += "\\t";
                        break;
                    default:
                        ret += c;
                        break;
                }
            }
            return ret + "\"";
        }

    }
}