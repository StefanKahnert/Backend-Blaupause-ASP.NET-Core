using System;


namespace Backend_Blaupause.Helper
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class ScheduleHourAttribute : ScheduleAttribute
    {

        public ScheduleHourAttribute(DayOfWeek day, int hour, int min, double interval) : base(day, hour, min, interval)
        {
            this.intervalType = Scheduler.HOURS;
        }

    }
}
