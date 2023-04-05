using System;

namespace Game.Utility
{
    public static class TimeUtility
    {
        public static TimeSpan GetRemainingTime(TimeSpan finishTime, TimeSpan currentTime)
        {
            var remaining = finishTime - currentTime;
            return remaining > TimeSpan.Zero ?
                remaining :
                TimeSpan.Zero;
        }

        public static TimeSpan CeilTo(TimeSpan time, TimeSpan to)
        {
            long ticks = (long)(Math.Ceiling(time.Ticks / (double)to.Ticks) * to.Ticks);
            return new TimeSpan(ticks);
        }

        public static TimeSpan FloorTo(TimeSpan time, TimeSpan to)
        {
            long ticks = (long)(Math.Floor(time.Ticks / (double)to.Ticks) * to.Ticks);
            return new TimeSpan(ticks);
        }

        public static TimeSpan RoundTo(TimeSpan time, TimeSpan to)
        {
            long ticks = (long)(Math.Round(time.Ticks / (double)to.Ticks) * to.Ticks);
            return new TimeSpan(ticks);
        }

        public static TimeSpan Ceil(TimeSpan time)
        {
            return
                time.TotalDays > 1 ?
                CeilTo(time, TimeSpan.FromDays(1)) :
                time.TotalHours > 1 ?
                CeilTo(time, TimeSpan.FromHours(1)) :
                time.TotalMinutes > 1 ?
                CeilTo(time, TimeSpan.FromMinutes(1)) :
                time;
        }
    }
}