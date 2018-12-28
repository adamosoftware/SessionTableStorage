using System;

namespace SessionTableStorage.Library.Interfaces
{
	/// <summary>
	/// Implement this on classes that may be persisted for a while, but then need to be automatically updated after some time passes
	/// </summary>
	public interface ITimedCacheable
	{
		/// <summary>
		/// UTC time of last update
		/// </summary>
		DateTime LastUpdate { get; set; }

		/// <summary>
		/// Data is considered stale after this number of minutes
		/// </summary>		
		int MaxLifetimeMinutes { get; }
	}
}