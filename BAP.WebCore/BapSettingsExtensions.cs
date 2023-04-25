using BAP.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BAP.WebCore
{
    public static class BapSettingsExtensions
    {
        public static string PackagePath(this BapSettings settings)
        {
            return Path.Combine(settings.AddonSaveLocation, "packages");
        }
    }
}
