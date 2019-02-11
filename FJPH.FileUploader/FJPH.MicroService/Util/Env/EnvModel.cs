using System;
using System.Collections.Generic;
using System.Text;

namespace FJPH.MicroService.Util.Env
{

    /// <summary>
    /// 输入的系统环境变量模型
    /// </summary>
    public class EnvModel : BaseModel
    {
        /// <summary>
        /// APP 构建 版本号
        /// </summary>
        public string APP_BUILD_NO { get; set; }

        /// <summary>
        /// APP构建 全名
        /// </summary>
        public string APP_BUILD_NAME { get; set; }

        /// <summary>
        ///  提供服务的URL 配置
        ///  例如 = "http://*:5555";
        /// </summary>
        public string ASPNETCORE_URLS { get; set; }

        /// <summary>
        /// 运行环境 Development|Staging| Production 用于获取不同的appconfig[env].json配置文件
        /// </summary>
        public string ASPNETCORE_ENVIRONMENT { get; set; }

        /// <summary>
        /// 服务注册中心地址
        /// </summary>
        public string APP_MICRO_SETVICE_REGISTRY_HOST { get; set; }

        /// <summary>
        /// 服务注册中心的数据中心名称
        /// </summary>
        public string APP_MICRO_SETVICE_REGISTRY_DATACENTER { get; set; }


        /// <summary>
        /// 应用程序集名称
        /// </summary>
        public string APP_ASSEMPLY_VERSION { get; set; }


    }


}
