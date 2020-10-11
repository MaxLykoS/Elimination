using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using UnityEngine;

public class Utility : Singletion<Utility>
{
    public static long GetTimeStamp()
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 10, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalSeconds);
    }
}
