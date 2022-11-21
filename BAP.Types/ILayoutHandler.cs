using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAP.Types
{
    public interface ILayoutHandler
    {
        ButtonLayout? SetNewButtonLayout(ButtonLayout? bl);
        ButtonLayout? CurrentButtonLayout { get; }
    }
}
