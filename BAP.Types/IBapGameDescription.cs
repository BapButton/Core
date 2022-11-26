using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAP.Types
{
    //Todo - Should this be replaced with an attribute on IBapGame? Would need to search through the addon ones as well as DI ones. 
    public interface IBapGameDescription : IMainAreaItem
    {
        /// <summary>
        /// Name of the game - Shown in the Menu
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Description of the Game - Also generally shown on the menu.
        /// </summary>
        public string Description { get; }

        public virtual string UniqueId
        {
            get
            {
                return this.GetType().FullName!;
            }
        }
    }
}