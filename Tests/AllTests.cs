using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
	[TestClass]
	public class AllTests
	{
		private MySession _session = new MySession("sampleUser");

		[TestMethod]
		public void ShouldSaveSimpleValue()
		{
			const string value = "hello";

			_session.SetAsync("greeting", value).Wait();

			string greeting = _session.GetAsync<string>("greeting").Result;

			Assert.AreEqual(value, greeting);
		}
	}
}
