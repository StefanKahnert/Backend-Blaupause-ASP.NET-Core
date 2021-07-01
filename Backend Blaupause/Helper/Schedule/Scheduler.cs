using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_Blaupause.Helper
{
    public class Scheduler
    {
        public static long SECONDS = 1;
        public static long MINUTES = 2;
        public static long HOURS = 3;
        public static long DAYS = 4;

        public static void Interval(DayOfWeek day, int hour, int minute, double interval, long intervalType, Action task)
        {
            DateTime now = DateTime.Now;

            if (intervalType == SECONDS)
            {
                IntervalInSeconds(day, hour, minute, interval, task);
            } else if (intervalType == MINUTES)
            {
                IntervalInMinutes(day, hour, minute, interval, task);
            }
            else if (intervalType == HOURS)
            {
                IntervalInHours(day, hour, minute, interval, task);
            }
            else if (intervalType == DAYS)
            {
                IntervalInDays(day, hour, minute, interval, task);
            }
        }

        public static void IntervalInSeconds(DayOfWeek day, int hour, int minute, double interval, Action task)
        {
            interval = interval / 3600;
            SchedulerService.Instance.ScheduleTask(day, hour, minute, interval, task);
        }
        public static void IntervalInMinutes(DayOfWeek day, int hour, int min, double interval, Action task)
        {
            interval = interval / 60;
            SchedulerService.Instance.ScheduleTask(day, hour, min, interval, task);
        }
        public static void IntervalInHours(DayOfWeek day, int hour, int min, double interval, Action task)
        {
            SchedulerService.Instance.ScheduleTask(day, hour, min, interval, task);
        }
        public static void IntervalInDays(DayOfWeek day, int hour, int min, double interval, Action task)
        {
            interval = interval * 24;
            SchedulerService.Instance.ScheduleTask(day, hour, min, interval, task);
        }
    }
}
