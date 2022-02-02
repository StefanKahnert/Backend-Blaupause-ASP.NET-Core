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
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class ScheduleSecondAttribute : ScheduleAttribute
    {

        public ScheduleSecondAttribute(DayOfWeek day = 0, int hour = 99, int min = 99, double interval = 0) : base(day, hour, min, interval)
        {
            this.intervalType = Scheduler.SECONDS;
        }

    }
}
