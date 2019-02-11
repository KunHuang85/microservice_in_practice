using FJPH.MicroService.Util.Env;
using System;
using System.Collections.Generic;
using System.Text;

namespace FJPH.MicroService.Logger.Model
{
    /// <summary>
    ///  日志对象内容
    /// </summary>
    public class LogMessageModel : BaseModel
    {
        /// <summary>
        /// ISO 8601 的字符串
        /// </summary>
        public string LogTime { get; set; }

        /// <summary>
        /// 日志隔离级别
        /// </summary>
        public string LogLevel { get; set; }

        /// <summary>
        /// 日志ID
        /// </summary>
        public string LogId { get; set; }

        /// <summary>
        /// 日志属性
        /// </summary>
        public string LogAttr { get; set; }

        /// <summary>
        /// 日志标签
        /// </summary>
        public string LogTag { get; set; }


        /// <summary>
        /// 消息内容
        /// </summary>
        public string LogMsg { get; set; }


        /// <summary>
        /// 日志类别
        /// </summary>
        public string LogCategory { get; set; }


        /// <summary>
        /// 生产外发字符串
        /// </summary>
        /// <returns></returns>

        public string ToLogstashMessage()
        {

            string strPublishMessage = $"<{this.LogTime}> <{this.LogLevel}> <{this.LogId}> <{this.LogAttr}> <{this.LogCategory}> ";
            strPublishMessage += $"<{ EnvHelper.GetEnv().APP_BUILD_NAME}> <{EnvHelper.GetLocalHostName()}> <{EnvHelper.GetLocalIPV4()}> <{this.LogMsg}>";

            return strPublishMessage;

        }


    }


}
