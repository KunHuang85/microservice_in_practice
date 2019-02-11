using FJPH.FileUploader.Controllers;
using FJPH.MicroService;
using FJPH.MicroService.Util.RabbitMQ;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FJPH.FileUploader.Jobs
{
    public class JobDefinition1 : IJob1
    {

        private readonly JobOption1 _jobOption;
        private readonly ILogger<TestConfigController> _logger;
        private readonly JobRabbitClient1 _jobRabbitClient;

        public JobDefinition1(JobOption1 jobOption, ILogger<TestConfigController> logger, JobRabbitClient1 jobRabbitClient)
        {
            _jobOption = jobOption;
            _logger = logger;
            _jobRabbitClient = jobRabbitClient;
        }
                       

        /// <summary>
        /// 做一个后台任务
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public Task DoTask(string s)
        {
            Task task = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("执行代码" + s);
                System.Diagnostics.Debug.WriteLine("执行代码" + s);
                _logger.LogDebug("执行代码" + s);

                    // 读 ORacle  List

                // 发送内容队列
                _jobRabbitClient.PublishMessage("内容aaaa||||" + _jobOption.JobName, "amq.topic", "msg.http.req.test");

            });






            return task;
        }


    }
}
