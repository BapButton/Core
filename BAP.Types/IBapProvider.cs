using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAP.Types
{
	public interface IBapProvider
	{
		string Name { get; }
		Task<bool> Initialize();
	}
}
