using Consul;
using FJPH.MicroService.Consul;
using FJPH.MicroService.Consul.Agent;
using FJPH.MicroService.Logger;
using FJPH.MicroService.Logger.RabbitMQ;
using FJPH.MicroService.Middleware;
using FJPH.MicroService.Util.Env;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;

namespace FJPH.MicroService
{
    public static class LoggerServiceExtension
    {

        public static ILoggerFactory DefaultLoggerFactory { get; set; }

        /// <summary>
        /// 使用RabbitMQ Logger 组件服务
        /// </summary>
        /// <param name="app"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="strConsulKey"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseRabbitMQLoggerService(this IApplicationBuilder app, ILoggerFactory loggerFactory, string strConsulKey = "comm/logger/rabbitmq")
        {
            DefaultLoggerFactory = loggerFactory;
            IConsulKVHelper _ConsulKVHelper = app.ApplicationServices.GetService<IConsulKVHelper>();

            try
            {
                // 从 Consul 中获取RabbitMQ 位置
                RabbitMQLoggerOption conf = _ConsulKVHelper.GetJsonModelAsync<RabbitMQLoggerOption>(strConsulKey).Result;
                // 添加LoggerFactory
                loggerFactory.AddRabbitMQLogger(conf);
            }
            catch (Exception ex)
            {
                _ConsulKVHelper.SetValueAsync(strConsulKey, "{}");
                throw new ArgumentException($"注册中心配置[{strConsulKey}]不存在或无法转化为json", ex);
            }

            return app;
        }

    }
}
