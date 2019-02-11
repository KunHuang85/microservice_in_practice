using FJPH.MicroService.Util.RabbitMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FJPH.FileUploader.Jobs
{

    /// <summary>
    /// 存在多个RabbitMQ服务器的情况或者需要多个RabbitMQClient实例的情况下
    /// 继承RabbitMQClient类，并在Startup 中注入，如下
    /// </summary>
    public class JobRabbitClient1 : RabbitMQClient
    {
        /// <summary>
        /// 构造函数，使用依赖注入，传递自定义的jobOption到Client 父类
        /// </summary>
        /// <param name="jobOption"></param>
        public JobRabbitClient1(JobOption1 jobOption) : base(jobOption)
        {

        }
    }
}
