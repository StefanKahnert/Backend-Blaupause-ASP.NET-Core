using System;


namespace Backend_Blaupause.Helper
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class ScheduleSecondAttribute : ScheduleAttribute
    {

        public ScheduleSecondAttribute(DayOfWeek day = 0, int hour = 99, int min = 99, double interval = 0) : base(day, hour, min, interval)
        {
            this.intervalType = Scheduler.SECONDS;
        }

    }
}
