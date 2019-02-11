using Consul;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Text;

namespace FJPH.MicroService.Scheduler
{
    public class SchedulerOption
    {
        private readonly IConsulKVHelper consulKVHelper;


        public bool UseDashboard { get; set; } = false;

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { get; set; } = false;

        /// <summary>
        /// 开启多少个工作者线程 Environment.ProcessorCount * 5
        /// </summary>
        public int WorkerCount { get; set; } = 1;


        /// <summary>
        /// 存储配置路径
        /// </summary>
        public string ConfigKey { get; set; } = "comm/database/postgresql/scheduler";

        /// <summary>
        /// 设置所使用的队列列表
        /// </summary>
        public List<string> QueueList { get; set; }


        /// <summary>
        /// 后台运行任务配置
        /// </summary>
        public BackgroundJobServerOptions BackgroundJobServerOptions { get; set; }



        /// <summary>
        /// 调度器数据库配置
        /// </summary>
        private SchedulerDbConfigModel SchedulerDbConfigModel { get; set; }


        public SchedulerOption(ConsulClient consulClient, Util.Env.EnvModel _EnvModel)
        {
            this.consulKVHelper = new ConsulKVHelper(consulClient);
            this.QueueList = new List<string>() { "share" };
            this.BackgroundJobServerOptions = new BackgroundJobServerOptions()
            {
                HeartbeatInterval = TimeSpan.FromSeconds(5),
                ServerCheckInterval = TimeSpan.FromSeconds(2),
                ServerName = _EnvModel.APP_BUILD_NAME,
                SchedulePollingInterval = TimeSpan.FromSeconds(1),
                WorkerCount = this.WorkerCount
            };
        }

        public SchedulerDbConfigModel LoadConfig()
        {

            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.WriteLine($"---------- [{DateTime.Now}] 加载调度器配置[SchedulerOption] 使用KvKey[{this.ConfigKey}]");
            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.WriteLine();


            this.SchedulerDbConfigModel = this.consulKVHelper.GetJsonModelAsync<SchedulerDbConfigModel>(this.ConfigKey).Result;
            this.BackgroundJobServerOptions.Queues = this.QueueList.ToArray();

            Console.WriteLine($"---------- [{DateTime.Now}] 加载调度器配置[SchedulerOption] Value=\n[{this.SchedulerDbConfigModel.ToString()}]");
            Console.WriteLine("--------------------------------------------------------------------------------");

            return this.SchedulerDbConfigModel;


        }


    }

}
