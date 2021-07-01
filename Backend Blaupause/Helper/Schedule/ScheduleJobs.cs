using Backend_Blaupause.Models;
using Backend_Blaupause.Models.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_Blaupause.Helper.Schedule
{
    public class ScheduleJobs
    {
        private readonly DatabaseContext db;

        private readonly ILogger<ScheduleJobs> logger;

        public ScheduleJobs(DatabaseContext db, ILogger<ScheduleJobs> logger) 
        {
            this.db = db;
            this.logger = logger;
        }


        [ScheduleSecond(interval: 10)]
        public void scheduleTest()
        {
            logger.LogInformation("Schedule Test run!");
        }

    }
}
