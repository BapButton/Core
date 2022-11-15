using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAP.Types
{
    public interface IGamePage
    {
        Task<bool> NodesChangedAsync(NodeChangeMessage nodeChangeMessage);
        Task<bool> LayoutChangedAsync();
        Task<bool> GameUpdateAsync(GameEventMessage gameEventMessage);
    }

}
