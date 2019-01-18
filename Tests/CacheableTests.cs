using Microsoft.VisualStudio.TestTools.UnitTesting;
using SessionTableStorage.Library.Enums;
using System.Threading.Tasks;
using Tests.Models;

namespace Tests
{
	[TestClass]
	public class CacheableTests
	{
		private MyCacheableStorage _storage = new MyCacheableStorage();

		[TestMethod]
		public void UserProfileShouldLoad()
		{
			// hypothetical current user
			const string currentUser = "adamo";
			var profile = _storage.GetAsync(currentUser, () => GetSampleProfile(currentUser)).Result;
			Assert.IsTrue(profile.UserName.Equals(currentUser));
		}

		[TestMethod]
		public void UserProfileShouldLoadFromCacheIfAvailable()
		{
			const string currentUser = "adamo";

			// manually add something to cache
			_storage.SetAsync(currentUser, GetSampleProfile(currentUser)).Wait();

			// access the cached item as you would in a real app
			var profile = _storage.GetAsync(currentUser, () => GetSampleProfile(currentUser)).Result;

			// after manually pushing to cache, it should retrieve that way
			Assert.IsTrue(profile.RetrievedFrom == RetrievedFrom.Cache);
		}

		[TestMethod]
		public void UserProfileShouldLoadLiveIfNotInCache()
		{
			const string currentUser = "adamo";

			// manually delete something from cache if present
			_storage.DeleteAsync(currentUser).Wait();

			// access the cached item as you would in a real app
			var profile = _storage.GetAsync(currentUser, () => GetSampleProfile(currentUser)).Result;

			// after manually deleting from cache, it should retrieve live
			Assert.IsTrue(profile.RetrievedFrom == RetrievedFrom.Live);
		}

		[TestMethod]
		public void UserProfileShouldInvalidateAndRetrieveLive()
		{
			const string currentUser = "adamo";

			var profile = _storage.GetAsync(currentUser, () => GetSampleProfile(currentUser)).Result;

			// simulate a change that a user would make on a Profile manage page, for instance
			profile.TimeZoneOffset = 6;
			profile.PhoneNumber = "111-232-3438";

			// in a real app, you would save the updated profile (for example with Postulate Save)			
			_storage.InvalidateAsync<UserProfile>(currentUser).Wait();			

			// simulates a new page request
			profile = _storage.GetAsync<UserProfile>(currentUser, () => profile).Result;

			Assert.IsTrue(profile.RetrievedFrom == RetrievedFrom.Live);
		}

		private async static Task<UserProfile> GetSampleProfile(string userName)
		{
			// in a real app, this would be a database query based on User.Identity.Name
			return new UserProfile()
			{
				UserName = userName,
				Email = "adamosoftware@gmail.com",
				TimeZoneOffset = -5,
				Permissions = 3423,
				PhoneNumber = "234-323-4899"
			};
		}
	}
}