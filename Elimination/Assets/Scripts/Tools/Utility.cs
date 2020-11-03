using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using UnityEngine;

public class Utility : Singletion<Utility>
{
    public static ToolRandom Random = new ToolRandom();
    public static long GetTimeStamp()
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 10, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalSeconds);
    }
}

public class ToolRandom
{
    private static ulong next = 1;
    public static void SetRandSeed(ulong seed)
    {
        next = seed;
    }

    public static int rand_10000()
    {
        next = next * 1103515245 + 12345;
        return (int)((next / 65536) % 1000);
    }
}
