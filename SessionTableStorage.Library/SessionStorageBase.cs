using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace SessionTableStorage.Library
{
	public abstract class SessionStorageBase
	{
		protected abstract CloudTable GetTable();

		private readonly string _partitionKey;

		public SessionStorageBase(string partitionKey)
		{
			_partitionKey = partitionKey;
		}

		public async Task SetAsync(string rowKey, object data)
		{
			var table = GetTable();
			var insertOrReplace = TableOperation.InsertOrReplace(new StorageEntity(_partitionKey, rowKey, data));
			await table.ExecuteAsync(insertOrReplace);
		}

		/// <summary>
		/// Queries the cloud table and returns the TableResult object
		/// </summary>
		public async Task<TableResult> QueryTableAsync(string rowKey)
		{
			var table = GetTable();
			var query = TableOperation.Retrieve<StorageEntity>(_partitionKey, rowKey);
			return await table.ExecuteAsync(query);
		}

		/// <summary>
		/// Queries the cloud table and returns the T-typed data.
		/// If exception occurs and defaultValue is set, then defaultValue is returned instead
		/// </summary>
		public async Task<T> GetAsync<T>(string rowKey, T defaultValue = default(T))
		{
			try
			{
				var result = await QueryTableAsync(rowKey);				
				string json = ((StorageEntity)result.Result).Json;
				return JsonConvert.DeserializeObject<T>(json);				
			}
			catch
			{
				if (!defaultValue.Equals(default(T))) return defaultValue;
				throw;
			}
		}

		public async Task Clear()
		{
			var table = GetTable();
			//var delete = TableOperation.
		}
	}
}