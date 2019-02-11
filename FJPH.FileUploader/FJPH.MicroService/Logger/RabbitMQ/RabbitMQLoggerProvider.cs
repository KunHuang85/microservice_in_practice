using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace FJPH.MicroService.Logger.RabbitMQ
{
    /// <summary>
    ///  RabbitMQLoggerProvider
    /// </summary>
    public class RabbitMQLoggerProvider : ILoggerProvider
    {
        protected RabbitMQLoggerOption RabbitMQLoggerConfig { get; set; }

        protected ConnectionFactory RabbitConnectionFactory { get; set; }

        protected IConnection RabbitConnection { get; set; }

        protected IModel RabbitChannel { get; set; }


        private readonly IConsulKVHelper consulKVHelper;


        public RabbitMQLoggerProvider(IConsulKVHelper consulKVHelper)
        {
            this.consulKVHelper = consulKVHelper;
        }

        /// <summary>
        /// 构造函数 -  使用IOption 
        /// </summary>
        /// <param name="options"></param>
        public RabbitMQLoggerProvider(IOptions<RabbitMQLoggerOption> options)
        {
            this.RabbitMQLoggerConfig = options.Value;
            InitRabbitConnection(this.RabbitMQLoggerConfig);
        }


        /// <summary>
        /// 构造函数 - 读取注册中心配置
        /// </summary>
        /// <param name="logConfig">RabbitMQLoggerConfig</param>
        public RabbitMQLoggerProvider(RabbitMQLoggerOption logConfig)
        {
            this.RabbitMQLoggerConfig = logConfig;
            InitRabbitConnection(this.RabbitMQLoggerConfig);
        }


        /// <summary>
        /// 新建 Logger，外部调用
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public ILogger CreateLogger(string categoryName)
        {
            RabbitMQLogger rabbitMQLogger = new RabbitMQLogger(this, categoryName);
            return rabbitMQLogger;
        }


        /// <summary>
        /// 初始化到 RabbitMQ 的连接
        /// </summary>
        private void InitRabbitConnection(RabbitMQLoggerOption rabbitMQLoggerConfig)
        {
            if (this.RabbitConnectionFactory == null)
            {
                this.RabbitConnectionFactory = new ConnectionFactory()
                {
                    HostName = rabbitMQLoggerConfig.Hostname,
                    UserName = rabbitMQLoggerConfig.UserName,
                    Password = rabbitMQLoggerConfig.Password,
                    Port = rabbitMQLoggerConfig.Port,
                    VirtualHost = rabbitMQLoggerConfig.VirtualHost
                };
            }

            if (this.RabbitConnection == null || !this.RabbitConnection.IsOpen)
            {
                this.RabbitConnection = this.RabbitConnectionFactory.CreateConnection();
            }

            if (this.RabbitChannel == null || this.RabbitChannel.IsClosed || !this.RabbitChannel.IsOpen)
            {
                this.RabbitChannel = this.RabbitConnection.CreateModel();
            }


            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.WriteLine($"---------- [{DateTime.Now}] 日志组件[RabbitMQLogProvider]]\t建立了到RabbitMQ服务的连接");
            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.WriteLine();
        }


        /// <summary>
        /// 消息内容，应用系统名称
        /// </summary>
        /// <param name="message"></param>
        /// <param name="appName"></param>
        public void PublishMessage(string message, string appName)
        {
            this.InitRabbitConnection(this.RabbitMQLoggerConfig);

            var body = Encoding.UTF8.GetBytes(message);

            // 实际的发送代码    
            this.RabbitChannel.BasicPublish(exchange: this.RabbitMQLoggerConfig.ExchangeName,
                                 routingKey: this.RabbitMQLoggerConfig.RoutingKey + appName,
                                 basicProperties: null,
                                 body: body);

            if (this.RabbitMQLoggerConfig.IsShowDebugInfo)
            {
                Console.WriteLine("--------------------------------------------------------------------------------");
                Console.WriteLine($"---------- [{DateTime.Now}] 外发日志\tExchange:{this.RabbitMQLoggerConfig.ExchangeName}");
                Console.WriteLine($"---------- [{DateTime.Now}] 外发日志\tRouteKey:{this.RabbitMQLoggerConfig.RoutingKey + appName}");
                Console.WriteLine($"---------- [{DateTime.Now}] 外发日志\tMessage:\n{message}");
                Console.WriteLine("--------------------------------------------------------------------------------");
                Console.WriteLine();
            }

        }

        public void Dispose()
        {
            this.RabbitChannel.Close();
            this.RabbitChannel.Dispose();
            this.RabbitConnection.Close();
            this.RabbitConnection.Dispose();
            this.RabbitConnectionFactory = null;

            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.WriteLine($"---------- [{DateTime.Now}] 日志组件[RabbitMQLogProvider]]\tDispose");
            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.WriteLine();
        }
    }
}
