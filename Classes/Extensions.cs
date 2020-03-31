using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public static class Extensions
{
    public static double ToUnixTime(this DateTime value)
    {
        double unixTimestamp = (value.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        return unixTimestamp * 1000;
    }
}
