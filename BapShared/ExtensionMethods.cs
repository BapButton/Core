using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BapShared
{
    public static class ExtensionMethods
    {
        static public void RaiseEvent(this EventHandler eh, object sender, EventArgs e)
        {
            if (eh != null)
                eh(sender, e);
        }

        static public void RaiseEvent<T>(this EventHandler<T> eh, object sender, T e)
            where T : EventArgs
        {
            if (eh != null)
                eh(sender, e);
        }
    }
}
