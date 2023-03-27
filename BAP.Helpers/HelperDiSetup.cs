using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAP.Helpers
{
    internal class HelperDiSetup : IDependencyInjectionSetup
    {
        public void AddItemsToDi(IServiceCollection services)
        {
            services.AddTransient<AnimationController>();
        }
    }
}
