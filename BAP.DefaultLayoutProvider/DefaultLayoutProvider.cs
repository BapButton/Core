using BAP.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAP.DefaultLayoutProvider
{
    [BapProvider("Default Layout Provider", "Provides a basic method for laying out buttons and retrieving that layout", "6420ba24-1f88-5823-a125-6de533f940e7")]
    public class DefaultLayoutProvider : ILayoutProvider
    {
        public ButtonLayout? CurrentButtonLayout { get; internal set; }

        public void Dispose()
        {

        }

        public Task<bool> InitializeAsync()
        {
            return Task.FromResult(true);
        }

        public ButtonLayout? SetNewButtonLayout(ButtonLayout? bl)
        {
            CurrentButtonLayout = bl;
            return bl;
        }


    }
}
