using System;
using System.Collections.Generic;

public static class TypeExtensions 
{
    public static int ToInt(this object o)
    {
        switch (o)
        {
            case null:
                return 0;
            case int i:
                return i;
            case uint u:
                return (int)u;
            case float f:
                return (int)f;
            case long l:
                return (int)l;
            case bool b:
                return b ? 1 : 0;
            default:
                double.TryParse(o.ToString(), out var dVal);
                return (int)dVal;
        }
    }

    public static int ToInt(this object obj, string key)
    {
        if (obj is not IDictionary<string, object> dict) return 0;
        return dict.TryGetValue(key, out var o) ? o.ToInt() : 0;
    }

    public static float ToFloat(this object o)
    {
        switch (o)
        {
            case null:
                return 0;
            case int i:
                return i;
            case uint u:
                return (float)u;
            case float f:
                return f;
            case long l:
                return (float)l;
            default:
                float.TryParse(o.ToString(), out var dVal);
                return (float)dVal;
        }
    }

    public static uint ToColor(this object o)
    {
        switch (o)
        {
            case null:
                return 0;
            case uint u:
                return u;
            case string str:
                uint.TryParse(str, out var result);
                return result;
            default:
                return (uint)o.ToInt();
        }
    }

    public static float ToFloat(this object obj, string key)
    {
        if (obj is not IDictionary<string, object> dict) return 0;
        if (dict.TryGetValue(key, out var o))
            return o.ToFloat();
        return 0;
    }

    public static long ToLong(this object val)
    {
        switch (val)
        {
            case null:
                return 0;
            case int i:
                return i;
            case uint u:
                return (long)u;
            case float f:
                return (long)f;
            case long l:
                return l;
            default:
                long.TryParse(val.ToString(), out var dVal);
                return dVal;
        }
    }


    public static long ToLong(this object obj, string key)
    {
        if (obj is not IDictionary<string, object> dict) return 0;
        return dict.TryGetValue(key, out var o) ? o.ToLong() : 0;
    }

    public static double ToDouble(this object val)
    {
        switch (val)
        {
            case null:
                return 0;
            case int i:
                return i;
            case uint u:
                return u;
            case float f:
                return (long)f;
            case long l:
                return l;
            case bool b:
                return b ? 1 : 0;
            default:
                double.TryParse(val.ToString(), out var dVal);
                return dVal;
        }
    }


    public static double ToDouble(this object obj, string key)
    {
        if (obj is not IDictionary<string, object> dict) return 0;
        return dict.TryGetValue(key, out var o) ? o.ToLong() : 0;
    }

    public static bool ToBool(this object o)
    {
        switch (o)
        {
            case null:
                return false;
            case bool b:
                return b;
            case string s:
            {
                return s is "True" or "true" or "yes";
            }
            default:
                return o.ToInt() != 0;
        }
    }
    public static Dynamic ToDynamic(this object obj, string key)
    {
        if (obj is not IDictionary<string, object> dict) return null;
        if (dict.TryGetValue(key, out var o))
            return o as Dynamic;
        return null;
    }
    public static bool ToBool(this object obj, string key)
    {
        if (obj is not IDictionary<string, object> dict) return false;
        return dict.TryGetValue(key, out var o) && o.ToBool();
    }

    public static string ToString(this object obj, string key)
    {
        if (obj is not IDictionary<string, object> dict) return null;
        return dict.TryGetValue(key, out var o) ? o?.ToString() : null;
    }

    public static List<T> ToList<T>(this IDictionary<string, object> dict, string key)
    {
        dict.TryGetValue(key, out object o);
        return o as List<T>;
    }

    public static List<int> ToIntList(this IDictionary<string, object> dict, string key)
    {
        dict.TryGetValue(key, out var o);
        var objList = (List<object>)o ?? new List<object>();
        var intList = new List<int>();
        objList.ForEach((obj) => intList.Add(obj.ToInt()));
        return intList;
    }

    public static object ToObject(this object obj, string key)
    {
        if (obj is not IDictionary<string, object> dict) return null;
        return dict.TryGetValue(key, out var o) ? o : null;
    }
    // Extension method, call for any object, eg "if (x.IsNumeric())..."
    public static bool IsNumeric(this object x) { return x != null && IsNumeric(x.GetType()); }

    // Method where you know the type of the object
    public static bool IsNumeric(Type type) { return IsNumeric(type, Type.GetTypeCode(type)); }

    // Method where you know the type and the type code of the object
    public static bool IsNumeric(Type type, TypeCode typeCode) { return typeCode == TypeCode.Decimal || (type.IsPrimitive && typeCode != TypeCode.Object && typeCode != TypeCode.Boolean && typeCode != TypeCode.Char); }
}