using BAP.Admin;
using BAP.Types;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAP.PrimaryHandlers
{
    public class AdminDiSetup : IDependencyInjectionSetup
    {
        public void AddItemsToDi(IServiceCollection services)
        {
            services.AddTransient<IValidator<FileUpload>, FileUploadValidator>();
            services.AddMemoryCache();
        }
    }
}
