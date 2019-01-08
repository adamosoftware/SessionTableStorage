using Microsoft.WindowsAzure.Storage.Table;
using SessionTableStorage.Library;

namespace Tests
{
	public class MySession : SessionStorageBase
	{
		public MySession(string partitionKey) : base(partitionKey)
		{
		}
		
		protected override CloudTable GetTable()
		{
			return CloudTableHelper.GetTable();
		}
	}
}