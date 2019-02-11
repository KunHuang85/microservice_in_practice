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
    public class TestConfigController : ControllerBase
    {
        public readonly IConfiguration Configuration;
        private readonly ILogger<TestConfigController> _logger;
        private StorageConfigModel _storageConfigModel;

        public TestConfigController(IConfiguration configuration, ILogger<TestConfigController> logger, StorageConfigModel storageConfigModel)
        {
            Configuration = configuration;
            _logger = logger;
            _storageConfigModel = storageConfigModel;
        }

        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            _logger.LogDebug("测试日志输出" + id);
            return Ok(Configuration.GetSection(id).Value +">>>>"+ _storageConfigModel.StoragePath);
        }




    }
}