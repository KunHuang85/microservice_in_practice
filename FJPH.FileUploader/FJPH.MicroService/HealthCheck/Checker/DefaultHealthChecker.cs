using Consul;
using FJPH.MicroService.Util.Env;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FJPH.MicroService.HealthCheck.Checker
{
    /// <summary>
    /// 默认的健康检查
    /// </summary>
    public class DefaultHealthChecker : IHealthCheck
    {
        private readonly EnvModel envModel;

        public DefaultHealthChecker(EnvModel envModel)
        {
            this.envModel = envModel;
        }


        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (envModel.ASPNETCORE_ENVIRONMENT == "Development" || envModel.ASPNETCORE_ENVIRONMENT == "Testing" || envModel.ASPNETCORE_ENVIRONMENT == "Production")
            {
                return Task.FromResult(HealthCheckResult.Healthy($"健康 : 应用环境变量{envModel.ASPNETCORE_ENVIRONMENT} 正确"));
            }
            else
            {
                return Task.FromResult(HealthCheckResult.Unhealthy($"不健康 : 应用环境变量{envModel.ASPNETCORE_ENVIRONMENT} 错误"));
            }
        }

    }
}
