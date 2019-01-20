using Microsoft.WindowsAzure.Storage.Table;
using SessionTableStorage.Library;
using System;
using System.Threading.Tasks;

namespace Tests
{
	public class MyTimedCacheableStorage : TimedCacheableStorage
	{
		protected override async Task<CloudTable> GetTableAsync()
		{
			return await CloudTableHelper.GetTableAsync();
		}
	}
}