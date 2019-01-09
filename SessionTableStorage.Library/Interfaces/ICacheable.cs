using SessionTableStorage.Library.Enums;

namespace SessionTableStorage.Library.Interfaces
{
	public interface ICacheable
	{
		/// <summary>
		/// This is the flag for determining whether the cached data is usable or not.
		/// ORMs should not map this property to a column because table data is always valid from a caching perspective
		/// </summary>
		bool IsValid { get; set; }

		/// <summary>
		/// Indicates how the object was retrieved, live or from cache.
		/// ORMs should not map this
		/// </summary>
		RetrievedFrom RetrievedFrom { get; set; }
	}
}