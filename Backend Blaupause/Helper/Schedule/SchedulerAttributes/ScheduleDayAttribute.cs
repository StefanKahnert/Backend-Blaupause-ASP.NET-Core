using System;


namespace Backend_Blaupause.Helper
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class ScheduleDayAttribute : ScheduleAttribute
    {

        public ScheduleDayAttribute(DayOfWeek day, int hour, int min, double interval) : base(day, hour, min, interval)
        {
            this.intervalType = Scheduler.DAYS;
        }

    }
}
