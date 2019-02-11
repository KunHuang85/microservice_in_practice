using FJPH.MicroService.Logger.Model;
using FJPH.MicroService.Util.Env;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace FJPH.MicroService.Logger.RabbitMQ
{
    /// <summary>
    /// 
    /// </summary>
    public class RabbitMQLogger : ILogger
    {
        private readonly RabbitMQLoggerProvider _RabbitMQLoggerProvider;
        private readonly string categoryName;
        private readonly EnvModel envModel;


        public RabbitMQLogger(RabbitMQLoggerProvider rabbitMQLoggerProvider, string categoryName)
        {
            // Console.WriteLine("RabbitMQLogger - INIT LOGGER");

            this._RabbitMQLoggerProvider = rabbitMQLoggerProvider;
            this.categoryName = categoryName;
            this.envModel = EnvHelper.GetEnv();
        }


        public IDisposable BeginScope<TState>(TState state)
        {
            // Console.WriteLine("RabbitMQLogger - BeginScope>>" + state);
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        /// <summary>
        /// 实际的调用方法
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="logLevel"></param>
        /// <param name="eventId"></param>
        /// <param name="state"></param>
        /// <param name="exception"></param>
        /// <param name="formatter"></param>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            string msg = formatter(state, exception);

            LogMessageModel logMessageModel = new LogMessageModel()
            {
                LogMsg = (exception == null) ? msg : new LogExceptionModel(exception, msg).ToString(),
                LogTime = DateTime.Now.ToString("o"),
                LogLevel = Enum.GetName(typeof(LogLevel), logLevel),
                LogId = Guid.NewGuid().ToString(),
                LogAttr = eventId.Name + eventId.Id,
                LogCategory = this.categoryName
            };

            string strPublishMessage = logMessageModel.ToLogstashMessage();

            this._RabbitMQLoggerProvider.PublishMessage(strPublishMessage, this.envModel.APP_BUILD_NAME);
        }
    }
}
