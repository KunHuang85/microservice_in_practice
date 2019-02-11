using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Text;

namespace FJPH.MicroService.HealthCheck.Result
{


    public class HealthCheckResponse : BaseModel
    {
        private readonly HealthReport healthReport;

        public HealthCheckResponse(HealthReport healthReport)
        {
            this.healthReport = healthReport;

            this.Checkers = new List<HealthCheckerModel>();
            this.Result = this.healthReport.Status.ToString();


            foreach (var item in this.healthReport.Entries)
            {
                HealthCheckerModel checkerModel = new HealthCheckerModel()
                {
                    Result = item.Value.Status.ToString(),
                    Name = item.Key,
                    Msg = item.Value.Description
                };

                this.Checkers.Add(checkerModel);
            }
        }

        /// <summary>
        ///  健康检查结果
        /// </summary>
        public string Result { get; private set; }


        /// <summary>
        ///  所有的checker
        /// </summary>
        public List<HealthCheckerModel> Checkers { get; private set; }


    }
}
