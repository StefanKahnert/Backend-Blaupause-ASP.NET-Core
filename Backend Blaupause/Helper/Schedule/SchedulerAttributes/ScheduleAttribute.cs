using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Backend_Blaupause.Models.Interfaces;
using System.Net;
using Backend_Blaupause.Helper.ExceptionHandling;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;


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
