using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Backend_Blaupause.Helper
{
    public class SchedulerService
    {
        private static SchedulerService _instance;
        private List<Timer> timers = new List<Timer>();
        private SchedulerService() { }
        public static SchedulerService Instance => _instance ?? (_instance = new SchedulerService());
        public void ScheduleTask(DayOfWeek day, int hour, int min, double intervalInHour, Action task)
        {
            DateTime now = DateTime.Now;

            DateTime firstRun = DateTime.Now;

            if (day == 0 && hour == 99 && min == 99) //wenn alle default => firstrun = jetzt
            {
                firstRun = now;
            } else if (day == 0 || hour == 99 || min == 99) //wenn nicht alle default => firstrun = jetzt + werte nicht-default
            {
                if (day == 0)
                {
                    day = now.DayOfWeek;
                }
                if (hour == 99)
                {
                    hour = now.Hour;
                }
                if (min == 99)
                {
                    min = now.Minute;
                }
            }         
            else
            {
                DateTime startDay = Next(now, day);
                firstRun = new DateTime(startDay.Year, startDay.Month, startDay.Day, hour, min, 0, 0);
            }

            
            if (now > firstRun)
            {
                firstRun = firstRun.AddDays(1);
            }
            TimeSpan timeToGo = firstRun - now;
            if (timeToGo <= TimeSpan.Zero)
            {
                timeToGo = TimeSpan.Zero;
            }
            var timer = new Timer(x =>
            {
                task.Invoke();
            }, null, timeToGo, TimeSpan.FromHours(intervalInHour));
            timers.Add(timer);
        }

        private static DateTime Next(DateTime from, DayOfWeek dayOfWeek)
        {
            int start = (int)from.DayOfWeek;
            int target = (int)dayOfWeek;
            if (target <= start)
                target += 7;
            return from.AddDays(target - start);
        }
    }
}
