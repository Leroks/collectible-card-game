using System.Collections.Generic;

public class Random
{
    int _seed;
     
    public Random() => _seed = new System.Random().Next();

    public Random(int seed) => _seed = seed;

    /** Return a random boolean value (true or false) */
    public bool Bool() => NextFloat() < 0.5;

    /** Return a random integer between 'from' and 'to', inclusive. */
    public int Int(int from, int to) => (int)(from + ((to - from) * NextFloat()) + .5f);

    /** Return a random float between 'from' and 'to', inclusive. */
    public float Float(float from, float to) => (float)(from + ((to - from) * NextFloat()));

    /** Return a random string of a certain length.  You can optionally specify 
        which characters to use, otherwise the default is (a-zA-Z0-9) */
    public string String(int length, string charactersToUse = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789")
    {
        var str = "";
        for (var i = 0; i < length; i++)
        {
            str += charactersToUse[Int(0, charactersToUse.Length - 1)];
        }
        return str;
    }

    public int NextInt()
    {
        _seed = 214013 * _seed + 2531011;
        return (_seed >> 16) & 0x7FFF;
    }

    public float NextFloat() => NextInt() / (float)short.MaxValue;

    public void Shuffle<T>(ref List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Int(0, n);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}