using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalPortal.BL
{
    public class JobSchedule
    {
        public static void Start()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();
            IJobDetail job1 = JobBuilder.Create<Job>().Build();
            ITrigger trigger1 = TriggerBuilder.Create()
                       .WithIdentity("trigger1", "group1")
              .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(11,55))
              .ForJob(job1)
              .Build();
            scheduler.ScheduleJob(job1, trigger1);
        }
    }
}