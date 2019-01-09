using Microsoft.WindowsAzure.Storage.Table;
using SessionTableStorage.Library;
using System;

namespace Tests
{
	public class MyTimedCacheableStorage : TimedCacheableStorage
	{
		protected override CloudTable GetTable()
		{
			return CloudTableHelper.GetTable();
		}
	}
}