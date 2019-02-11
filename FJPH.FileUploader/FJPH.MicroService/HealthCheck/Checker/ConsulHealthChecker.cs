using Consul;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FJPH.MicroService.HealthCheck.Checker
{
    public class ConsulHealthChecker : IHealthCheck
    {
        private readonly IConsulKVHelper consulKVHelper;

        public ConsulHealthChecker(IConsulKVHelper consulKVHelper)
        {
            this.consulKVHelper = consulKVHelper;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                string[] keys = consulKVHelper.GetKvKeys("").Result.Response;
                return Task.FromResult(HealthCheckResult.Healthy($"健康 : 微服务连接配置中心正常,KV存储发现了[{keys.Length}]个Key"));
            }
            catch (Exception ex)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy($"不健康 : 微服务连接配置中心异常,{ex.Message}"));
            }

        }
    }
}
