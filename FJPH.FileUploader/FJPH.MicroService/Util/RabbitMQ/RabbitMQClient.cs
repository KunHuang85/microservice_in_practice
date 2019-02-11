using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace FJPH.MicroService.Util.RabbitMQ
{
    /// <summary>
    /// RabbitMQ 启动配置
    /// </summary>
    public class RabbitMQClient
    {
        public RabbitMQConnectionOption RabbitMQConnectionOption { get; set; }

        public ConnectionFactory RabbitConnectionFactory { get; set; }

        public IConnection RabbitConnection { get; set; }

        public IModel RabbitChannel { get; set; }

        /// <summary>
        /// 是否使用调试模式
        /// </summary>
        public bool IsDebugModel { get; set; } = false;


        public RabbitMQClient(RabbitMQConnectionOption rabbitMQConnectionOption)
        {
            this.RabbitMQConnectionOption = rabbitMQConnectionOption;
        }

        /// <summary>
        /// 建立到获取到 RabbitMQ 的连接
        /// </summary>
        public IModel InitRabbitConnection()
        {
            if (this.RabbitConnectionFactory == null)
            {
                this.RabbitConnectionFactory = new ConnectionFactory()
                {
                    HostName = RabbitMQConnectionOption.Hostname,
                    UserName = RabbitMQConnectionOption.UserName,
                    Password = RabbitMQConnectionOption.Password,
                    Port = RabbitMQConnectionOption.Port,
                    VirtualHost = RabbitMQConnectionOption.VirtualHost
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

            return this.RabbitChannel;

        }



        /// <summary>
        /// 消息内容，应用系统名称
        /// </summary>
        /// <param name="message"></param>
        /// <param name="appName"></param>
        public void PublishMessage(string message, string exchangename, string routekey)
        {
            this.InitRabbitConnection();

            var body = Encoding.UTF8.GetBytes(message);

            // 实际的发送代码    
            this.RabbitChannel.BasicPublish(exchange: exchangename,
                                 routingKey: routekey,
                                 basicProperties: null,
                                 body: body);

            if (this.IsDebugModel)
            {
                Console.WriteLine("--------------------------------------------------------------------------------");
                Console.WriteLine($"---------- [{DateTime.Now}] 外发队列\tExchange:{exchangename}");
                Console.WriteLine($"---------- [{DateTime.Now}] 外发队列\tRouteKey:{routekey}");
                Console.WriteLine($"---------- [{DateTime.Now}] 外发队列\tMessage:\n{message}");
                Console.WriteLine("--------------------------------------------------------------------------------");
                Console.WriteLine();
            }

        }

    }
}
