using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FJPH.MicroService.Logger.Model
{
    public class LogExceptionModel : BaseModel
    {
        public LogExceptionModel(Exception ex, string state)
        {
            this.StateMessage = state;
            this.ExceptionMessage = ex.Message;
            this.ExceptionSource = ex.Source;
            this.ExceptionStackTrace = ex.StackTrace;
            this.ExceptionDeclaringType = ex.TargetSite.DeclaringType.ToString();
        }

        public string StateMessage { get; set; }
        public string ExceptionMessage { get; set; }
        public string ExceptionSource { get; set; }
        public string ExceptionStackTrace { get; set; }
        public string ExceptionDeclaringType { get; set; }
    }
}
