using System;
using System.Collections.Generic;
using System.Text;

namespace FJPH.MicroService.Scheduler
{
    public class SchedulerDbConfigModel : BaseModel
    {
        /// <summary>
        /// 数据库版本
        /// </summary>
        public string DbVersion { get; set; }

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

    }
}
