using System;
using System.Collections.Generic;

public class Map<TKey, TValue> : Dictionary<TKey, TValue>
{
    bool IsNull(TKey val) => val is not ValueType && null == val;
    bool IsNull(TValue val) => val is not ValueType && null == val;
     
    public new TValue this[TKey key]
    {
        get
        {
            if (IsNull(key))
                return default;
            return TryGetValue(key, out var result) ? result : default;
        }
        set
        {
            if (!IsNull(key))
            {
                if (IsNull(value))
                {
                    if (ContainsKey(key))
                        Remove(key);
                }
                else base[key] = value;
            }
        }
    }

    public Dynamic ToDynamic(TKey key) => this[key] as Dynamic;
    public int ToInt(TKey key) => this[key].ToInt();
    public float ToFloat(TKey key) => this[key].ToFloat();
}

public class Dynamic : Map<string, object>
{
    public Dynamic AddAll(Dynamic source)
    {
        using var iter = source.GetEnumerator();
        while (iter.MoveNext())
            this[iter.Current.Key] = iter.Current.Value;
        return this;
    }
    public Dynamic Clone() => new Dynamic().AddAll(this);
}