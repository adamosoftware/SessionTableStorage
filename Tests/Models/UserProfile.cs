using Newtonsoft.Json;
using SessionTableStorage.Library.Enums;
using SessionTableStorage.Library.Interfaces;

namespace Tests.Models
{
	/// <summary>
	/// Hypothetical user profile class
	/// </summary>
	internal class UserProfile : ICacheable
	{
		public string UserName { get; set; }
		public string Email { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string PhoneNumber { get; set; }
		public int TimeZoneOffset { get; set; }
		public long Permissions { get; set; }
		
		public bool IsValid { get; set; }

		[JsonIgnore]
		public RetrievedFrom RetrievedFrom { get; set; }

		public void Invalidate()
		{			
			new MyCacheableStorage().Invalidate<UserProfile>(UserName);
		}
	}
}