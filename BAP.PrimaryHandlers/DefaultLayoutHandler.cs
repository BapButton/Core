using BAP.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAP.PrimaryHandlers
{
    public class DefaultLayoutHandler : ILayoutHandler
    {
        public ButtonLayout? CurrentButtonLayout { get; internal set; }
        public ButtonLayout? SetNewButtonLayout(ButtonLayout? bl)
        {
            CurrentButtonLayout = bl;
            return bl;
        }
    }
}
