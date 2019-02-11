using FJPH.MicroService.Util.Env;
using System;
using System.Collections.Generic;
using System.Text;

namespace FJPH.MicroService.Logger.RabbitMQ
{
    /// <summary>
    /// Rabbit MQ 对象, 使用该类反序列化json
    /// </summary>
    public class RabbitMQLoggerOption : BaseModel
    {
        /// <summary>
        /// 是否启动调试信息
        /// </summary>
        public bool IsShowDebugInfo { get; set; }
        

        #region  Rabbit MQ Coifig

        public String LoggerName { get; set; }

        public String Hostname { get; set; }

        public String UserName { get; set; }

        public String Password { get; set; }

        public int Port { get; set; }

        public String VirtualHost { get; set; }

        public String ExchangeName { get; set; }

        public string RoutingKey { get; set; }


        #endregion



    }
}
