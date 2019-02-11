using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FJPH.FileUploader.Jobs;
using FJPH.FileUploader.Model;
using FJPH.MicroService.Scheduler;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FJPH.FileUploader.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestJobController : ControllerBase
    {
        public readonly IConfiguration Configuration;
        private readonly ILogger<TestConfigController> _logger;
        private readonly IJob1 _job1;

        private StorageConfigModel _storageConfigModel;


        public TestJobController(IConfiguration configuration, ILogger<TestConfigController> logger, IJob1 job1, StorageConfigModel storageConfigModel)
        {
            _storageConfigModel = storageConfigModel;
            Configuration = configuration;
            _logger = logger;
            _job1 = job1;
        }


        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            // 不支持匿名函数
            //  Expression<Func<Task>> methoCall1 = () => Task.Factory.StartNew((s) => Console.WriteLine("task1" + s), "aaaa");
                       

            Expression<Func<Task>> methoCall3 = () => _job1.DoTask("22222222222");

            SchedulerHelper.AddOrUpdateJob("jobId3", methoCall3, "17 12 * * *", "ivr_yzx");


            return Ok(id);
        }







    }
}