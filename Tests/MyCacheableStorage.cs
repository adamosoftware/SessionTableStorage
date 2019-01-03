using Microsoft.WindowsAzure.Storage.Table;
using SessionTableStorage.Library;
using System;

namespace Tests
{
	public class MyCacheableStorage : CacheableStorageBase
	{
		protected override bool ReturnNullIfNotExists => true;

		protected override CloudTable GetTable()
		{
			return CloudTableHelper.GetTable();
		}
	}
}