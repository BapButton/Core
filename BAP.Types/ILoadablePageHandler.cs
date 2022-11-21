using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAP.Types
{
    public interface ILoadablePageHandler
    {
        IMainAreaItem? CurrentlySelectedItem { get; set; }
        bool SelectedItemIsAGame { get; }
        bool UpdateCurrentlySelectedItem(IMainAreaItem mainAreaItem, bool createNewGameIfSameTypeLoaded = false);
    }
}
