using System;
using System.Collections.Generic;
using System.Text;

namespace FJPH.MicroService.Util.RabbitMQ
{
    /// <summary>
    ///     RabbitMQ 配置
    /// </summary>
    public class RabbitMQConnectionOption
    {
        public string Hostname { get; set; }

        public int Port { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string VirtualHost { get; set; }

    }
}
