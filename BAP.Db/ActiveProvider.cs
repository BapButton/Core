using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAP.Db
{
	public class ActiveProvider
	{
		public int ActiveProviderId { get; set; }
		public ProviderType ProviderType { get; set; }
		public DateTime DateActivated { get; set; }
		public string FullName { get; set; } = "";
	}

	public enum ProviderType
	{
		Unknown = 0,
		Audio = 1,
		Keyboard = 2,
		Button = 3
	}
}
