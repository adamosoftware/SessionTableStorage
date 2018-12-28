using DevSecrets.Library;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;

namespace Tests
{
	public static class CloudTableHelper
	{
		public static CloudTable GetTable()
		{
			var secrets = DevSecretsDictionary.Load("SessionTableStorage.sln");
			var creds = new StorageCredentials(secrets.Contents["name"], secrets.Contents["key"]);
			var account = new CloudStorageAccount(creds, true);
			var client = account.CreateCloudTableClient();
			var table = client.GetTableReference("SessionStorage");
			table.CreateIfNotExists();
			return table;
		}
	}
}