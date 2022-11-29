using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAP.Types
{
    public interface IMainAreaItem
    {
        /// <summary>
        /// This is the type of the Blazor Component that should be dynamically loaded in the main Body of the App.
        /// </summary>
        public Type DynamicComponentToLoad { get; }
    }
}
