using Consul;
using FJPH.MicroService.Util.Env;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace FJPH.MicroService
{
    /// <summary>
    /// 配置文件扩展
    /// </summary>
    public static class ConfigurationExtension
    {
        /// <summary>
        /// 带默认路径的配置文件扩展
        /// </summary>
        /// <typeparam name="TOptions">配置json model类</typeparam>
        /// <param name="services">IServiceCollection</param>
        /// <param name="sectionPath">配置节点名称</param>
        /// <returns></returns>

        public static IServiceCollection ConfigFromCenter<TOptions>(this IServiceCollection services, string sectionPath = "default") where TOptions : class
        {
            Console.WriteLine($"Build Config from Center section=[{sectionPath}]");

            EnvModel envModel = EnvHelper.GetEnv();

            string strAppKVPath = $"apps/{envModel.APP_BUILD_NAME}/{envModel.ASPNETCORE_ENVIRONMENT}/{sectionPath}";
            //string strFullPath = string.IsNullOrEmpty(sectionPath) ? strAppKVPath : $"{strAppKVPath}/{sectionPath}";
            return ConfigFromCenterWithFullPath<TOptions>(services, strAppKVPath);
        }



        /// <summary>
        /// 根据Path读取注册中心，反序列化Model ，使用依赖注入使用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceCollection"></param>
        /// <param name="strCenterConfigurationPath">注册中心配置路径</param>
        /// <returns></returns>
        public static IServiceCollection ConfigFromCenterWithFullPath<TOptions>(this IServiceCollection services, string strCenterConfigurationPath) where TOptions : class
        {
            Console.WriteLine($"Build Config from Center Full Path=[{strCenterConfigurationPath}]");

            EnvModel _EnvModel = EnvHelper.GetEnv();
            ConsulClient consulClient = new ConsulClient(x =>
            {
                x.Address = new Uri(_EnvModel.APP_MICRO_SETVICE_REGISTRY_HOST);
                x.Datacenter = _EnvModel.APP_MICRO_SETVICE_REGISTRY_DATACENTER;
            });

            IConsulKVHelper consulKVHelper = new ConsulKVHelper(consulClient);

            try
            {
                TOptions t = consulKVHelper.GetJsonModelAsync<TOptions>(strCenterConfigurationPath).Result;
                services.AddSingleton(t);

                Console.WriteLine("--------------------------------------------------------------------------------");
                Console.WriteLine($"---------- [{DateTime.Now}] 注册中心注册配置 PATH={strCenterConfigurationPath}");
                Console.WriteLine($"---------- [{DateTime.Now}] 注册中心注册配置 Type={typeof(TOptions)}");
                Console.WriteLine($"---------- [{DateTime.Now}] 注册中心注册配置 Value=\n{t.ToString()}");
                Console.WriteLine("--------------------------------------------------------------------------------");
                Console.WriteLine();

            }
            catch (Exception ex)
            {
                consulKVHelper.SetValueAsync(strCenterConfigurationPath, "{}");
                throw new ArgumentException($"注册中心配置[{strCenterConfigurationPath}]不存在或非JSON格式json", ex);
            }

            return services;
        }
    }
}
