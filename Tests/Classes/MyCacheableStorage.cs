using Microsoft.WindowsAzure.Storage.Table;
using SessionTableStorage.Library;

namespace Tests
{
	public class MyCacheableStorage : CacheableStorage
	{
		public MyCacheableStorage() : base("app-cacheable")
		{
		}

		protected override CloudTable GetTable()
		{
			return CloudTableHelper.GetTable();
		}
	}
}