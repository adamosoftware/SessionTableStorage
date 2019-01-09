using SessionTableStorage.Library.Enums;

namespace SessionTableStorage.Library.Interfaces
{
	/// <summary>
	/// Implement this on classes that can persist in Azure Table Storage indefinitely until they're deleted or marked invalid
	/// </summary>
	public interface ICacheable
	{
		/// <summary>
		/// This is the flag for determining whether the cached data is usable or not.
		/// ORMs should not map this property to a column because table data is always valid from a caching perspective;
		/// however always make sure this is serialized
		/// </summary>
		bool IsValid { get; set; }

		/// <summary>
		/// Indicates how the object was retrieved, live or from cache.
		/// ORMs should not map this, and should also never be serialized (use [JsonIgnore])
		/// </summary>
		RetrievedFrom RetrievedFrom { get; set; }

		/// <summary>
		/// Use this to delete or mark a cached object as invalid
		/// </summary>
		void Invalidate();
	}
}