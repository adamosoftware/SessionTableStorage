using DevSecrets.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Octokit;
using System.Linq;
using Tests.Models;

namespace Tests
{
	[TestClass]
	public class TimedCacheableTests
	{
		private MyTimedCacheableStorage _storage = new MyTimedCacheableStorage();

		[TestMethod]
		public void GithubActivityShouldRefresh()
		{
			var secrets = DevSecretsDictionary.Load("SessionTableStorage.sln");

			var data = _storage.GetAsync("activity", async () =>
			{
				string user = secrets.Contents["name"];
				var client = new GitHubClient(new ProductHeaderValue(user));
				client.Credentials = new Credentials(secrets.Contents["github-token"]);
				var events = await client.Activity.Events.GetAllUserPerformedPublic(user, new ApiOptions()
				{
					StartPage = 1,
					PageCount = 1,
					PageSize = 30
				});

				var array = events.ToArray();
				var flattened = array.Select(e => new GithubActivityFlattened(e)).ToArray();

				return new GithubActivityView() { Activities = flattened };
			}).Result;
		}

		[TestMethod]
		public void BaseGetMethodShouldFail()
		{
			try
			{
				var result = _storage.GetAsync<string>("whatever").Result;
				Assert.Fail("should not reach this");
			}
			catch 
			{
				//success
			}
		}
	}
}