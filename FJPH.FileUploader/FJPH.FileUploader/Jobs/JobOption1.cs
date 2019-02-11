using FJPH.MicroService.Util.RabbitMQ;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FJPH.FileUploader.Jobs
{
    /// <summary>
    /// 工作任务配置类参数：包含了公共配置文件的RabbitMQ配置
    /// 继承公共配置Model，额外的属性使用[JsonIgnore] 标签
    /// </summary>
    public class JobOption1 : RabbitMQConnectionOption
    {
        /// <summary>
        /// 自定义一个额外的属性
        /// </summary>

        [JsonIgnore]
        public String JobName { get; set; } = "定时工作内容";

    }
}
