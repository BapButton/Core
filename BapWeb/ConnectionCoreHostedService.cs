using BapDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BapShared;

namespace BapWeb
{
    public class ConnectionCoreHostedService : IHostedService

    {

        ControlHandler CtrlHandler { get; set; }

        public ConnectionCoreHostedService(ILogger<BapButton.TinkerConnectionCore> logger, ControlHandler ctrlHandler)
        {
            CtrlHandler = ctrlHandler;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Migrating the database");
            using ButtonContext db = new();
            try
            {
                db.Database.Migrate();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error migrating the database {ex.Message} Inner exception {(ex?.InnerException?.Message ?? "")}");
            }
            var mosquittoAddress = Environment.GetEnvironmentVariable("MosquittoAddress");
            if (CtrlHandler.CurrentController != null)
            {
                bool succesfullyInitialized = false;
                if (string.IsNullOrEmpty(mosquittoAddress))
                {
                    succesfullyInitialized = await CtrlHandler.CurrentController.Initialize();
                }
                else
                {
                    succesfullyInitialized = await CtrlHandler.CurrentController.Initialize(mosquittoAddress);
                }
                if(succesfullyInitialized == false)
                {
                    ///Todo: Somehow start up the mock controller. 
                    //CtrlHandler.CurrentController = new MockButtonCore.MockConnectionCore();
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }

}
