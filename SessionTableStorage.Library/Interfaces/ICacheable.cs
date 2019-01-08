using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SessionTableStorage.Library.Interfaces
{
	public interface ICacheable
	{
		/// <summary>
		/// This is the flag for determining whether the cached data is usable or not
		/// </summary>
		bool IsValid { get; set; }
	}
}
