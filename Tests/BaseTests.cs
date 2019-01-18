using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Linq;
using Tests.Models;

namespace Tests
{
	[TestClass]
	public class BaseTests
	{
		private MySession _session = new MySession("sampleUser");

		[TestMethod]
		public void SaveSimpleValue()
		{
			const string value = "hello";

			_session.SetAsync("greeting", value).Wait();

			string greeting = _session.GetAsync<string>("greeting").Result;

			Assert.AreEqual(value, greeting);
		}

		[TestMethod]
		public void SaveComplexType()
		{
			const string rowKey = "complexType";
			SampleType sample = GetSampleComplexType();

			_session.SetAsync(rowKey, sample).Wait();

			var getSample = _session.GetAsync<SampleType>(rowKey).Result;

			Assert.AreEqual(sample, getSample);
		}

		private static SampleType GetSampleComplexType()
		{
			return new SampleType()
			{
				OrderId = "KF34223",
				Description = "whatever is this description",
				Flag = true,
				DateValue = new DateTime(2019, 1, 1),
				Items = new NestedType[]
				{
					new NestedType() { ItemName = "thingus", Quantity = 34m, UnitPrice = 12.2m },
					new NestedType() { ItemName = "globulor", Quantity = 78m, UnitPrice = 8.23m },
					new NestedType() { ItemName = "prelcen", Quantity = 3.2m, UnitPrice = 34.9m }
				}
			};
		}

		[TestMethod]
		public void MissingValueShouldFail()
		{
			try
			{
				var missing = _session.GetAsync<string>("MissingValue").Result;
				Assert.Fail("Should not reach this");
			}
			catch
			{
				// success
			}
		}

		[TestMethod]
		public void DefaultValueShouldWork()
		{
			var defaultVal = _session.GetAsync("MissingValue", "dog").Result;
			Assert.AreEqual(defaultVal, "dog");
		}

		[TestMethod]
		public void ClearShouldLeaveNothing()
		{
			// need some sample value just to make sure there's something to clear
			_session.SetAsync("value", "whatever").Wait();
			_session.SetAsync("date", DateTime.Now).Wait();
			_session.ClearAsync().Wait();
			Assert.IsTrue(!_session.GetAllEntitiesAsync().Result.Any());
		}

		[TestMethod]
		public void SaveComplexTypeWithCustomSerializer()
		{
			const string rowKey = "indented";

			var sample = GetSampleComplexType();
			_session.SetAsync(rowKey, sample, (obj) => JsonConvert.SerializeObject(obj, new JsonSerializerSettings()
			{
				Formatting = Formatting.Indented,
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore
			})).Wait();

			var getSample = _session.GetAsync<SampleType>(rowKey).Result;
			Assert.AreEqual(sample, getSample);
		}

		[TestMethod]
		public void DeleteShouldWork()
		{
			const string deleteTest = "deleteTest";

			_session.SetAsync(deleteTest, GetSampleComplexType()).Wait();
			_session.DeleteAsync(deleteTest).Wait();

			var test = _session.GetAsync<SampleType>(deleteTest).Result;
			Assert.IsTrue(test == null);
		}

		[TestMethod]
		public void DeleteAsyncShouldWork()
		{
			const string deleteTest = "deleteTest";

			_session.SetAsync(deleteTest, GetSampleComplexType()).Wait();
			_session.DeleteAsync(deleteTest).Wait();

			var test = _session.GetAsync<SampleType>(deleteTest).Result;
			Assert.IsTrue(test == null);
		}
	}
}