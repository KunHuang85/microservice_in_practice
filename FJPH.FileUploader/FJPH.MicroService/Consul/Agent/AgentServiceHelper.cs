using Consul;
using FJPH.MicroService.Util.Env;
using System;

namespace FJPH.MicroService.Consul.Agent
{
    /// <summary>
    /// 服务注册与反注册实现类
    /// </summary>
    public class AgentServiceHelper : IAgentServiceHelper
    {
        public AgentServiceRegistration BuildMicroServiceRegisterInfo(AgentServiceOption conf)
        {
            var serviceRegistration = new AgentServiceRegistration
            {
                //对外公布的IP地址
                Address = EnvHelper.GetLocalIPV4(),
                Port = EnvHelper.GetServicePort(),
                Check = new AgentServiceCheck
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(conf.Service_Deregister_TimeSpan_Seconds),
                    Interval = TimeSpan.FromSeconds(conf.Service_HealthCheck_Interval_Seconds),
                    HTTP = conf.Service_HealthCheck_Route,
                    Timeout = TimeSpan.FromSeconds(conf.Service_Check_Timeout_Seconds)
                },

                ID = conf.Service_ID,
                Name = conf.Service_Name,
                Tags = conf.Service_TagList.ToArray()
            };

            return serviceRegistration;
        }


    }
}
