using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BAP.Types
{
    public interface IDependencyInjectionSetup
    {
        void AddItemsToDi(IServiceCollection services);
    }
}
