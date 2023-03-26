using System;


namespace Backend_Blaupause.Helper
{
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
    public class ScheduleAttribute : Attribute
    {
        public DayOfWeek day { get; set; }
        public int hour { get; set; }
        public int min { get; set; }
        public double interval { get; set; }
        public long intervalType { get; set; }

        public ScheduleAttribute(DayOfWeek day, int hour, int min, double interval)
        {
            this.day = day;
            this.hour = hour;
            this.min = min;
            this.interval = interval;
        }

    }
}
