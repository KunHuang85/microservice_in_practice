using Consul;
using System;
using System.Collections.Generic;
using System.Text;

namespace FJPH.MicroService.Consul.Agent
{
    public interface IAgentServiceHelper
    {
        /// <summary>
        /// 构建服务注册的模型信息
        /// </summary>
        /// <param name="conf"></param>
        /// <returns></returns>
        AgentServiceRegistration BuildMicroServiceRegisterInfo(AgentServiceOption conf);

    }
}
