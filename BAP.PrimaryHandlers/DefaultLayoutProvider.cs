using BAP.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAP.PrimaryHandlers
{
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
