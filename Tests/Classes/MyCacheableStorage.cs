using Microsoft.WindowsAzure.Storage.Table;
using SessionTableStorage.Library;
using System.Threading.Tasks;

namespace Tests
{
	public class MyCacheableStorage : CacheableStorage
	{
		public MyCacheableStorage() : base("app-cacheable")
		{
		}

		protected override async Task<CloudTable> GetTableAsync()
		{
			return await CloudTableHelper.GetTableAsync();
		}
	}
}