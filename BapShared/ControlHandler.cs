using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace BapShared
{
    public class ControlHandler
    {
        public List<(Type type, string Name)> TinkerControllerTypes { get; init; }
        private IServiceProvider Services { get; set; }
        public ControlHandler(IEnumerable<ITinkerController> allTinkerControllers, IServiceProvider services)
        {
            //I should probable inject the db context so I can get the default type.
            TinkerControllerTypes = allTinkerControllers.Select(t => (t.GetType(), t.ControllerName)).ToList();
            Services = services;
            foreach (var controller in allTinkerControllers)
            {
                if (controller.GetType()?.FullName?.Contains("Mock") == false)
                {
                    CurrentController = controller;
                }
                else
                {
                    if (controller != null)
                    {
                        controller.Dispose();
                    }

                }
            }
            if (CurrentController == null)
            {
                CurrentController = (ITinkerController)Services.GetRequiredService(TinkerControllerTypes.First().type);
            }
        }

        public ITinkerController CurrentController { get; set; }
        public bool ChangeTinkerController(Type type)
        {
            IEnumerable<ITinkerController> allControllers = Services.GetServices<ITinkerController>();
            foreach (var controller in allControllers)
            {
                if (controller.GetType() == type)
                {
                    CurrentController.Dispose();
                    CurrentController = controller;
                    CurrentController.Initialize();
                }
                else
                {
                    controller.Dispose();
                }
            }
            return true;
        }
    }
}
