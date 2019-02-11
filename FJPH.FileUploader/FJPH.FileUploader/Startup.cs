using FJPH.FileUploader.Jobs;
using FJPH.FileUploader.Model;
using FJPH.MicroService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FJPH.FileUploader
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.

        public void ConfigureServices(IServiceCollection services)
        {
            // 使用微软自带的IConfigration . Development对应配置文件appsettings.Development.json，Development 为本地环境
            // 其他显示发布环境则对应注册中心的配置 apps/应用名/环境名/AppSettingFile 下json内容，与默认的文件型配置文件结构一致
            services.AddSingleton(Configuration);

            // 某个应用的某个业务 注册中心配置文件依赖注入，自动补全路径
            services.ConfigFromCenter<StorageConfigModel>("Storage");


            //  添加服务注册与服务发现
            services.AddMicroService(
                // 服务注册配置
                agentServiceOption: agentOption =>
                {
                    agentOption.AddServiceTag("0123456789ABC");
                    // 健康监测配置一般采用默认，以下不需要配置
                    //agentOption.Service_Deregister_TimeSpan_Seconds = 20;// 默认不需要设置，健康检查失败到移除服务的时间
                    //agentOption.Service_HealthCheck_Interval_Seconds = 10; // 默认不需要设置，健康检查时间周期
                    //agentOption.Service_HealthCheck_Url =
                    // $"http://{EnvHelper.GetLocalIPV4()}:{EnvHelper.GetServicePort()}/api/health"; // 默认不要设置健康监测url
                },
                // 定时任务服务配置
                schedulerOption: schedulerOption =>
                {
                    schedulerOption.IsEnable = false;
                    schedulerOption.QueueList.Add("ivr_yzx");
                }
            );

            services.ConfigFromCenter<StorageConfigModel>("Storage");
            
            // 等价于上一个语句
            services.ConfigFromCenterWithFullPath<StorageConfigModel>("apps/sz-file-uploader-kun/Testing/Storage");
            
            // 多个应用业务共同依赖的某个相同配置，写全路径，例如使用公共配置节中的RabbitMQ
            // 如果多个任务使用同样的配置，使用类继承，额外的属性则使用[JsonIgnore] 标签
            services.ConfigFromCenterWithFullPath<JobOption1>("comm/messager/rabbitmq");

            // 注入job1 所使用的组件 RabbitMQ 客户端1
            services.AddSingleton<JobRabbitClient1>();

            //注入 job1 的接口和业务定义，使用Add Transient
            services.AddTransient<IJob1, JobDefinition1>();


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lf, ILoggerFactory loggerFactory)
        {
            // 微服务使用生命周期，管道的第一条，内置注册和默认健康检查
            app.UseMicroService(lf);               

            //  使用分布式日志系统
            app.UseRabbitMQLoggerService(loggerFactory);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseHangfireDashboard("");
            }

            app.UseMvc();


        }
    }
}
