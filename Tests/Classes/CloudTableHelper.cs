using DevSecrets.Library;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;

namespace Tests
{
	public static class CloudTableHelper
	{
		public static async Task<CloudTable> GetTableAsync()
		{
			var secrets = DevSecretsDictionary.Load("SessionTableStorage.sln");
			var creds = new StorageCredentials(secrets.Contents["name"], secrets.Contents["key"]);
			var account = new CloudStorageAccount(creds, true);
			var client = account.CreateCloudTableClient();
			var table = client.GetTableReference("SessionStorage");
			await table.CreateIfNotExistsAsync();
			return table;
		}
	}
}