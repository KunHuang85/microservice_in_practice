using System;
using System.Collections.Generic;
using System.Text;
using FJPH.MicroService.Logger.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace FJPH.MicroService.Logger
{
    /// <summary>
    ///  日志Provider 扩展
    /// </summary>
    public static class LoggerServiceExtension
    {

        #region RabbitMQ Logger Extension

        /// <summary>
        /// Rabbit MQ 配置扩展，使用完善的配置文件作为参数
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <param name="logOption"></param>
        /// <returns></returns>
        public static ILoggerFactory AddRabbitMQLogger(this ILoggerFactory loggerFactory, RabbitMQLoggerOption logOption)
        {
            RabbitMQLoggerProvider logProvider = new RabbitMQLoggerProvider(logOption);
            loggerFactory.AddProvider(logProvider);

            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.WriteLine($"---------- [{DateTime.Now}] 日志组件[RabbitMQLogConfig]\n{logOption.ToString()}");
            Console.WriteLine($"---------- [{DateTime.Now}] 日志组件[RabbitMQLogProvider]\t注册成功");
            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.WriteLine();

            return loggerFactory;
        }


        /// <summary>
        ///  Rabbit MQ 配置扩展，使用Lambda 表达式 Action
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <param name="configActionOption"></param>
        /// <returns></returns>
        public static ILoggerFactory AddRabbitMQLogger(this ILoggerFactory loggerFactory, Action<RabbitMQLoggerOption> configActionOption)
        {
            RabbitMQLoggerOption logOption = new RabbitMQLoggerOption();
            configActionOption(logOption);

            RabbitMQLoggerProvider logProvider = new RabbitMQLoggerProvider(logOption);
            loggerFactory.AddProvider(logProvider);

            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.WriteLine($"---------- [{DateTime.Now}] 日志组件[RabbitMQLogConfig]\n{logOption.ToString()}注册成功");
            Console.WriteLine($"---------- [{DateTime.Now}] 日志组件[RabbitMQLogProvider]\t注册成功");
            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.WriteLine();

            return loggerFactory;
        }

        #endregion

    }
}
