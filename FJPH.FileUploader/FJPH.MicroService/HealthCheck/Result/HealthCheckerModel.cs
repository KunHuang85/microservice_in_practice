using System;
using System.Collections.Generic;
using System.Text;

namespace FJPH.MicroService.HealthCheck.Result
{
    public class HealthCheckerModel
    {
        public string Name { get; set; }

        public string Result { get; set; }

        public string Msg { get; set; }
    }
}
