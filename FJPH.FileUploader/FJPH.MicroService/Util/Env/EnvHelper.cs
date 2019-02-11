using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace FJPH.MicroService.Util.Env
{

    /// <summary>
    ///  用于获取系统环境变量
    /// </summary>
    public static class EnvHelper
    {
        /// <summary>
        ///  获取系统环境变量
        /// </summary>
        /// <returns></returns>
        public static EnvModel GetEnv()
        {
            string STR_APP_BUILD_NAME = Environment.GetEnvironmentVariable("APP_BUILD_NAME");

            if (string.IsNullOrEmpty(STR_APP_BUILD_NAME))
            {
                throw new ArgumentException("ENV APP_BUILD_NAME IS NULL");
            }

            if (STR_APP_BUILD_NAME.Contains('\\')
                || STR_APP_BUILD_NAME.Contains('[')
                || STR_APP_BUILD_NAME.Contains(']')
                || STR_APP_BUILD_NAME.Contains('.')
                )
            {
                throw new ArgumentException("ENV APP_BUILD_NAME Has Invalid Charactor");
            }

            string STR_ASPNETCORE_ENVIRONMENT = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            if (string.IsNullOrEmpty(STR_ASPNETCORE_ENVIRONMENT))
            {
                throw new ArgumentException("ENV ASPNETCORE_ENVIRONMENT IS NULL");
            }


            EnvModel envModel = new EnvModel()
            {
                APP_BUILD_NAME = STR_APP_BUILD_NAME,

                APP_BUILD_NO = Environment.GetEnvironmentVariable("APP_BUILD_NO"),

                // 注册中心地址 
                APP_MICRO_SETVICE_REGISTRY_HOST = Environment.GetEnvironmentVariable("APP_MICRO_SETVICE_REGISTRY_HOST"),
                APP_MICRO_SETVICE_REGISTRY_DATACENTER = Environment.GetEnvironmentVariable("APP_MICRO_SETVICE_REGISTRY_DATACENTER"),

                // Dotnet Core 环境变量
                ASPNETCORE_ENVIRONMENT = STR_ASPNETCORE_ENVIRONMENT,
                ASPNETCORE_URLS = Environment.GetEnvironmentVariable("ASPNETCORE_URLS"),

                // 应用程序集合信息
                APP_ASSEMPLY_VERSION = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()
            };

            return envModel;
        }

        /// <summary>
        /// 获取服务端口
        /// </summary>
        /// <returns></returns>
        public static int GetServicePort()
        {
            EnvModel envModel = GetEnv();
            int port;
            try
            {
                port = int.Parse(envModel.ASPNETCORE_URLS.Split(":")[2]);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("环境变量 [ASPNETCORE_URLS] 格式错误", ex);
            }

            return port;
        }

        /// <summary>
        /// 获取本机IP
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIPV4()
        {
            ///获取本地的IP地址
            string AddressIP = string.Empty;
            foreach (IPAddress _IPAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
                {
                    AddressIP = _IPAddress.ToString();
                }
            }

            return AddressIP;
        }

        /// <summary>
        ///   通过网络DNS获取本机Host名
        /// </summary>
        /// <returns></returns>
        public static string GetLocalHostName()
        {
            return Dns.GetHostName();
        }

        /// <summary>
        /// 通过环境变量获取机器名称
        /// </summary>
        /// <returns></returns>
        public static string GetMachineName()
        {
            return Environment.MachineName;
        }


        /// <summary>
        /// 获取操作系统名称 Linux | Windows | OSX | UNKNOWN
        /// </summary>
        /// <returns></returns>
        public static string GetOS()
        {

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "Linux";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "Windows";

            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return "OSX";
            }
            else
            {
                return "UNKNOWN";
            }

        }



    }
}
