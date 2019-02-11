using FJPH.MicroService.Consul.Agent;
using FJPH.MicroService.Util.Env;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FJPH.MicroService.Middleware
{

    /// <summary>
    /// 健康检查中间件
    /// </summary>
    public class DefaultHealthCheckMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly EnvModel _envModel;
        private readonly AgentServiceOption _agentServiceOption;

        /// <summary>
        /// 消息服务中间件
        /// </summary>
        /// <param name="next"></param>
        /// <param name="agentServiceOption"></param>
        /// <param name="envModel"></param>
        public DefaultHealthCheckMiddleware(RequestDelegate next, AgentServiceOption agentServiceOption, EnvModel envModel)
        {
            _next = next;
            _agentServiceOption = agentServiceOption;
            _envModel = envModel;
        }

        /// <summary>
        /// 实现 健康监测 middleware 方法
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.Value.Equals(this._agentServiceOption.Service_HealthCheck_Route))
            {
                string msg = $"Instance Health: OK <br/>" +
                    $"Instance Container IP:[{ EnvHelper.GetLocalIPV4()}] <br/>" +
                    $"Instance Service URL:[{_envModel.ASPNETCORE_URLS}] <br/>" +
                    $"MiceoService Version[{_envModel.APP_ASSEMPLY_VERSION}],[{_envModel.ASPNETCORE_ENVIRONMENT}]";

                //Content-Encoding
                context.Response.StatusCode = 200;
                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync(msg);

            }
            else
            {
                await _next(context);
            }

        }
    }
}
