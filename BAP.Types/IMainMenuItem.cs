using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAP.Types
{
	public interface IMainMenuItem : IMainAreaItem
	{
		/// <summary>
		/// Shown in the Menu
		/// </summary>
		public string MenuItemName { get; }
	}
}
