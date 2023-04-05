using Game.Utility;
using System;

public static class TimeExtensions
{
    public static DateTime ChangeTime(this DateTime dateTime, int hours, int minutes, int seconds, int milliseconds = 0)
    {
        return ChangeTime(dateTime, new TimeSpan(hours, minutes, seconds, milliseconds));
    }

    public static DateTime ChangeTime(this DateTime dateTime, TimeSpan timeSpan)
    {
        return dateTime.Date + timeSpan;
    }

    public static TimeSpan Ceil(this TimeSpan ts)
    {
        return TimeUtility.Ceil(ts);
    }

    public static TimeSpan GetTimeLeft(this TimeSpan finishTime, TimeSpan currentTime)
    {
        return TimeUtility.GetRemainingTime(finishTime, currentTime);
    }
}