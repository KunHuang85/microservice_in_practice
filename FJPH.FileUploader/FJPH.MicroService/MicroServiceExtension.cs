using Consul;

using FJPH.MicroService.Consul.Agent;
using FJPH.MicroService.HealthCheck.Checker;
using FJPH.MicroService.HealthCheck.Result;
using FJPH.MicroService.Scheduler;
using FJPH.MicroService.Util.Env;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MsHealthStatus = Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus;

namespace FJPH.MicroService
{
    /// <summary>
    ///  微服务使用扩展
    /// </summary>
    public static class MicroServiceExtension
    {
        static EnvModel _EnvModel = EnvHelper.GetEnv();


        /// <summary>
        ///  添加微服务组件
        /// </summary>
        /// <param name="services">扩展 IServiceCollection</param>
        /// <param name="agentServiceOption">服务注册</param>
        /// <param name="healthCheckOptionAction">服务发现健康检查</param>
        /// <returns></returns>
        public static MicroServiceComponent AddMicroService(this IServiceCollection services,
            Action<AgentServiceOption> agentServiceOption = null,
            Action<SchedulerOption> schedulerOption = null)
        {
            // 包含的服务组件
            MicroServiceComponent microServiceComponent = new MicroServiceComponent
            {
                IHealthChecksBuilder = services.AddHealthChecks(),
                IServiceCollection = services
            };

            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.WriteLine($"---------- [{DateTime.Now}] 系统架构：{RuntimeInformation.OSArchitecture}");
            Console.WriteLine($"---------- [{DateTime.Now}] 系统名称：{RuntimeInformation.OSDescription}");
            Console.WriteLine($"---------- [{DateTime.Now}] 进程架构：{RuntimeInformation.ProcessArchitecture}");
            Console.WriteLine($"---------- [{DateTime.Now}] 是否64位操作系统：{Environment.Is64BitOperatingSystem}");
            Console.WriteLine("--------------------------------------------------------------------------------");

            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.WriteLine($"---------- [{DateTime.Now}] 初始化微服务组件\t应用名称：[{_EnvModel.APP_BUILD_NAME}]");
            Console.WriteLine($"---------- [{DateTime.Now}] 初始化微服务组件\t应用环境：[{_EnvModel.ASPNETCORE_ENVIRONMENT}]");
            Console.WriteLine($"---------- [{DateTime.Now}] 初始化微服务组件\t微服务程序集版本：[V{_EnvModel.APP_ASSEMPLY_VERSION}]");
            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.WriteLine();

            TimeZoneInfo timeZoneInfo = TimeZoneInfo.Local;
            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.WriteLine($"---------- [{DateTime.Now}] 时区信息 timeZoneInfo.DisplayName\t：[{timeZoneInfo.DisplayName}]");
            Console.WriteLine($"---------- [{DateTime.Now}] 时区信息 timeZoneInfo.Id\t：[{timeZoneInfo.Id}]");
            Console.WriteLine($"---------- [{DateTime.Now}] 时区信息 timeZoneInfo.StandardName\t：[{timeZoneInfo.StandardName}]");
            Console.WriteLine($"---------- [{DateTime.Now}] 时区信息 timeZoneInfo.DaylightName\t：[{timeZoneInfo.DaylightName}]");
            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.WriteLine();


            #region 添加 Consul 服务到管道

            // 单例模式注入 系统环境变量
            services.AddSingleton(_EnvModel);

            ConsulClient consulClient = new ConsulClient(x =>
            {
                x.Address = new Uri(_EnvModel.APP_MICRO_SETVICE_REGISTRY_HOST);
                x.Datacenter = _EnvModel.APP_MICRO_SETVICE_REGISTRY_DATACENTER;
            });

            // 单例模式注入 ConsulClient
            services.AddSingleton(consulClient);

            // Consul 相关服务注入
            services.AddSingleton<IConsulKVHelper, ConsulKVHelper>();
            services.AddSingleton<IAgentServiceHelper, AgentServiceHelper>();

            #endregion

            #region 配置Option的初始化

            // 加载选项的默认值
            AgentServiceOption _agentServiceOption = new AgentServiceOption(_EnvModel);
            SchedulerOption _schedulerOption = new SchedulerOption(consulClient, _EnvModel);

            // 获取外部定义配置，为空则取以上默认值
            agentServiceOption?.Invoke(_agentServiceOption);
            schedulerOption?.Invoke(_schedulerOption);

            // 配置的依赖注入
            services.AddSingleton(_agentServiceOption);
            services.AddSingleton(_schedulerOption);

            #endregion


            #region 添加服务的健康检查


            microServiceComponent.IHealthChecksBuilder.AddCheck<DefaultHealthChecker>("default_health_check",
                failureStatus: MsHealthStatus.Degraded,
                tags: new[] { "microservice", "env" });


            microServiceComponent.IHealthChecksBuilder.AddCheck<ConsulHealthChecker>("consul_health_check",
               failureStatus: MsHealthStatus.Unhealthy,
               tags: new[] { "microservice", "consul" });


            #endregion


            // 添加外部应用 Hangfire 使用
            if (_schedulerOption.IsEnable)
            {
                Console.WriteLine("--------------------------------------------------------------------------------");
                Console.WriteLine($"---------- [{DateTime.Now}] 调度器组件服务初始化\t调度器名称：[{_EnvModel.APP_BUILD_NAME}]");

                SchedulerDbConfigModel schedulerDbConfigModel = _schedulerOption.LoadConfig();
                services.AddHangfire(x => x.UsePostgreSqlStorage(schedulerDbConfigModel.ConnectionString));

                Console.WriteLine($"---------- [{DateTime.Now}] 调度器组件服务初始化成功]");
                Console.WriteLine("--------------------------------------------------------------------------------");
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("--------------------------------------------------------------------------------");
                Console.WriteLine($"---------- [{DateTime.Now}] 调度器组件不使用");
                Console.WriteLine("--------------------------------------------------------------------------------");
                Console.WriteLine();
            }
            return microServiceComponent;
        }


        /// <summary>
        /// 使用 系统启动可以自动注册以及关闭时候反注册服务
        /// </summary>
        /// <param name="app"></param>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseMicroService(this IApplicationBuilder app, IApplicationLifetime lifetime)
        {
            // 从管道中获取 环境变量配置
            EnvModel envModel = app.ApplicationServices.GetService<EnvModel>();

            // 从管道中获取 Consul客户端
            var consulClient = app.ApplicationServices.GetService<ConsulClient>();

            // 从管道中获取 获取 微服务配置
            AgentServiceOption _agentServiceOption = app.ApplicationServices.GetService<AgentServiceOption>();

            // 从管道中获取 获取服务注册的服务实例  
            IAgentServiceHelper _AgentServiceBuilder = app.ApplicationServices.GetService<IAgentServiceHelper>();
            IConsulKVHelper _ConsulKVHelper = app.ApplicationServices.GetService<IConsulKVHelper>();


            // 中间件1 : 健康检查
            app.UseHealthChecks(_agentServiceOption.Service_HealthCheck_Route, new HealthCheckOptions()
            {
                ResultStatusCodes =
                {
                    [MsHealthStatus.Healthy] = StatusCodes.Status200OK,
                    [MsHealthStatus.Degraded] = StatusCodes.Status200OK,
                    [MsHealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                },
                AllowCachingResponses = false,
                ResponseWriter = WriteResponse
            });


            #region 服务注册

            // 构建 服务注册 信息
            AgentServiceRegistration serviceRegistration = _AgentServiceBuilder.BuildMicroServiceRegisterInfo(_agentServiceOption);


            // 取消注册（使用生命周期）
            lifetime.ApplicationStopping.Register(() =>
            {
                consulClient.Agent.ServiceDeregister(serviceRegistration.ID).Wait();
                Console.WriteLine("--------------------------------------------------------------------------------");
                Console.WriteLine($"---------- [{DateTime.Now}] 服务[{serviceRegistration.ID}]：反注册成功");
                Console.WriteLine("--------------------------------------------------------------------------------");
                Console.WriteLine();
            });

            // 添加 注册(先反注册再注册)
            consulClient.Agent.ServiceRegister(serviceRegistration).Wait();

            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.WriteLine($"---------- [{DateTime.Now}]  服务[{serviceRegistration.ID}]：注册成功");
            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.WriteLine();


            // 添加健康检查 管道
            app.UseMiddleware<HealthCheckMiddleware>();
            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.WriteLine($"---------- [{DateTime.Now}] 注册：健康检查中间件启动");
            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.WriteLine();

            #endregion




            #region 添加HangFire组件
            SchedulerOption schedulerOption = app.ApplicationServices.GetService<SchedulerOption>();
            if (schedulerOption.IsEnable)
            {
                Console.WriteLine("--------------------------------------------------------------------------------");
                Console.WriteLine($"---------- [{DateTime.Now}] 启动调度器服务 ----------");

                app.UseHangfireServer(options: schedulerOption.BackgroundJobServerOptions);//启动Hangfire服务

                if (schedulerOption.UseDashboard)
                {
                    Console.WriteLine($"---------- [{DateTime.Now}] 启动调度器管理页面 ----------");
                    app.UseHangfireDashboard();
                }

                Console.WriteLine("--------------------------------------------------------------------------------");
            }

            #endregion

            return app;
        }


        /// <summary>
        ///  健康检查报告
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private static Task WriteResponse(HttpContext httpContext, HealthReport result)
        {
            httpContext.Response.ContentType = "application/json";

            HealthCheckResponse healthCheckResponse = new HealthCheckResponse(result);
            var msg = healthCheckResponse.ToString();
            return httpContext.Response.WriteAsync(msg);
        }

    }
}

