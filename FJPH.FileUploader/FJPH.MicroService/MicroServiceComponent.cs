using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace FJPH.MicroService
{
    public class MicroServiceComponent
    {
        public IHealthChecksBuilder IHealthChecksBuilder { get; set; }

        public IServiceCollection IServiceCollection { get; set; }

    }
}
