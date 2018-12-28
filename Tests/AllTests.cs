using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Models;

namespace Tests
{
	[TestClass]
	public class AllTests
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

			var sample = new SampleType()
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

			_session.SetAsync(rowKey, sample).Wait();

			var getSample = _session.GetAsync<SampleType>(rowKey).Result;

			Assert.AreEqual(sample, getSample);
		}
	}
}
