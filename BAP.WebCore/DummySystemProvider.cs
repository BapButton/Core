using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAP.WebCore
{
    [BapProvider("Web App System Provider", "Attempts to manage lifecycle a little bit with code.", uniqueId: "7a09f100-967c-49ea-b3cf-af03e2ba3f78")]
    public class WebAPPSystemProvider : ISystemProvider
    {
        IApplicationLifetime _applicationLifetime;
        public WebAPPSystemProvider(IApplicationLifetime applicationLifetime)
        {
            _applicationLifetime = applicationLifetime;
        }

        public void Dispose()
        {

        }

        public async Task<bool> InitializeAsync()
        {
            Console.WriteLine("Using the Dummy System Provider. It does not reboot or restart anything. You will have to do that yourself.");
            return await Task.FromResult(true);
        }

        public async Task<bool> RebootSystem()
        {
            _applicationLifetime.StopApplication();
            return await Task.FromResult(true);
        }

        public async Task<bool> RebootWebApp()
        {
            _applicationLifetime.StopApplication();
            return await Task.FromResult(true);
        }

        public async Task<bool> RebootWifi()
        {
            Console.WriteLine("Rebooting Wifif????");
            return await Task.FromResult(true);
        }

        public async Task<bool> ShutdownSystem()
        {
            _applicationLifetime.StopApplication();
            return await Task.FromResult(true);
        }
    }
}
