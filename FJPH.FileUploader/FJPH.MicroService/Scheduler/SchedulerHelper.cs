using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FJPH.MicroService.Scheduler
{
    public static class SchedulerHelper
    {

        /// <summary>
        /// 添加后台工作任务
        /// </summary>
        /// <param name="recurringJobId"></param>
        /// <param name="methodCall"></param>
        /// <param name="cronExpression"></param>
        /// <param name="queue"></param>
        /// <param name="timeZone"></param>
        public static void AddOrUpdateJob(string recurringJobId, Expression<Func<Task>> methodCall, string cronExpression, string queue = "share", TimeZoneInfo timeZone = null)
        {
            if (timeZone == null)
            {
                timeZone = TimeZoneInfo.Local;
            }

            RecurringJob.AddOrUpdate(recurringJobId, methodCall, cronExpression, timeZone: timeZone, queue: queue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="recurringJobId"></param>
        /// <param name="methodCall"></param>
        /// <param name="cronExpression"></param>
        /// <param name="timeZone"></param>
        /// <param name="queue"></param>
        public static void AddOrUpdateJob<T>(string recurringJobId, Expression<Func<T, Task>> methodCall, string cronExpression, TimeZoneInfo timeZone = null, string queue = "share")
        {
            if (timeZone == null)
            {
                timeZone = TimeZoneInfo.Local;
            }


            //FindSystemTimeZoneById("China Standard Time");
            RecurringJob.AddOrUpdate<T>(recurringJobId, methodCall, cronExpression, timeZone: timeZone, queue: queue);
        }


    }
}
