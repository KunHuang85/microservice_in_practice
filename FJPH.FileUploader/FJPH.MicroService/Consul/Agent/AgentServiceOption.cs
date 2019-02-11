using Consul;
using FJPH.MicroService.Util.Env;
using System;
using System.Collections.Generic;
using System.Text;

namespace FJPH.MicroService.Consul.Agent
{
    /// <summary>
    /// 服务注册与反注册所需要的配置文件
    /// </summary>
    public class AgentServiceOption
    {
        private readonly EnvModel _envModel;

        /// <summary>
        /// 构造函数- 默认值
        /// </summary>
        public AgentServiceOption(EnvModel envModel)
        {
            _envModel = envModel;

            // 标签
            Service_TagList = new List<string>() { ".Net Core 2.2", _envModel.ASPNETCORE_ENVIRONMENT, _envModel.APP_BUILD_NO };

            // 服务标签与服务名
            Service_ID = $"{EnvHelper.GetLocalIPV4().Replace(".", "")}";
            Service_Name = $"{envModel.APP_BUILD_NAME}".ToLower();

        }

        #region 服务注册和反注册所使用的属性

        /// <summary>
        /// 注册中心健康第一次应用检查失败后，到自动执行反注册操作的时间间隔
        /// </summary>
        public int Service_Deregister_TimeSpan_Seconds { get; set; } = 30;

        /// <summary>
        /// 健康检查时间周期，每x秒向指定url发起请求检查，http返回值200则注册中心认为正常
        /// </summary>
        public int Service_HealthCheck_Interval_Seconds { get; set; } = 5;

        /// <summary>
        /// 健康监测的路由url地址 默认 api/health，一般不需要实现
        /// </summary>
        public string Service_HealthCheck_Route { get; set; } = "/api/health";

        /// <summary>
        /// 服务注册超时时间,默认10秒，等待健康检查返回结果的等待超时
        /// </summary>
        public int Service_Check_Timeout_Seconds { get; set; } = 5;

        /// <summary>
        /// 该服务的ID
        /// </summary>
        public string Service_ID { get; private set; }

        /// <summary>
        /// 服务名称
        /// </summary>
        public string Service_Name { get; private set; }


        /// <summary>
        /// 自定义标签列表
        /// </summary>
        public List<string> Service_TagList { get; private set; }


        #endregion


        #region 服务注册和反注册所使用的方法

        /// <summary>
        /// 添加一个服务标签，字符串长度最大为10
        /// </summary>
        /// <param name="tagName">一个服务标签</param>
        public void AddServiceTag(string tagName)
        {
            if (tagName.Length > 10)
            {
                tagName = tagName.Remove(10);
            }

            this.Service_TagList.Add(tagName);
        }

        #endregion


    }
}
