﻿using BAP.PrimayHandlers;
using BAP.Types;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAP.PrimaryHandlers
{
    public class PrimaryHandlerDiSetup : IDependencyInjectionSetup
    {
        public void AddItemsToDi(IServiceCollection services)
        {
            //services.AddSingleton<IGameHandler, DefaultGameHandler>();
            //services.AddSingleton<DefaultGameHandler>();
            //services.AddSingleton<ILoadablePageHandler, DefaultLoadablePageHandler>();
            //services.AddSingleton<DefaultLoadablePageHandler>();
            //services.AddSingleton<ILayoutHandler, DefaultLayoutHandler>();
            //services.AddSingleton<DefaultLayoutHandler>();
            //services.AddSingleton<IKeyboardHandler, DefaultKeyboardHandler>();
            //services.AddSingleton<DefaultKeyboardHandler>();
            //services.AddSingleton<IControlHandler, ControlHandler>();
            //services.AddSingleton<ControlHandler>();
        }
    }
}