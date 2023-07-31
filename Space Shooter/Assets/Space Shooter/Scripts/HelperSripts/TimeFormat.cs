using System;
using UnityEngine;

public static class TimeFormat
{
    public static string Format(int seconds)
    {
        TimeSpan ts = TimeSpan.FromSeconds(seconds);

        if (ts.Days != 0) return ts.Days + "d " + ts.Hours + "h " + ts.Minutes + "m " + ts.Seconds + "s";
        else if (ts.Hours != 0) return ts.Hours.ToString("D1") + ":" + ts.Minutes.ToString("D2") + ":" + ts.Seconds.ToString("D2");
        else if (ts.Minutes != 0) return ts.Minutes.ToString("D1") + ":" + ts.Seconds.ToString("D2");
        else if (ts.Seconds != 0) return "0:" + ts.Seconds.ToString("D2");
        else if (ts.Seconds == 0) return "0:00";
        else
        {
            Debug.LogWarning("TimeFormat: seconds < 0");
            return "";
        }
    }
}
