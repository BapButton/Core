using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace BAP.WebCore
{
    internal class BapSettingsFakeOptionSnapshot : IOptionsSnapshot<BapSettings>
    {
        BapSettings _bapSettings;
        internal BapSettingsFakeOptionSnapshot(BapSettings bapSettings)
        {
            _bapSettings = bapSettings;
        }
        public BapSettings Value => _bapSettings;

        public BapSettings Get(string? name)
        {
            return _bapSettings;
        }
    }
}
