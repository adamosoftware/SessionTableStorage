using Microsoft.WindowsAzure.Storage.Table;
using SessionTableStorage.Library;
using System.Threading.Tasks;

namespace Tests
{
	public class MySession : SessionStorageBase
	{
		public MySession(string partitionKey) : base(partitionKey)
		{
		}
		
		protected override async Task<CloudTable> GetTableAsync()
		{
			return await CloudTableHelper.GetTableAsync();
		}
	}
}