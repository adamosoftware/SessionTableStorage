using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

		public async Task ClearAsync()
		{
			var table = GetTable();
			var entities = await GetAllEntitiesAsync(table);
			foreach (var e in entities)
			{
				var delete = TableOperation.Delete(e);
				await table.ExecuteAsync(delete);
			}
		}

		public async Task<IEnumerable<StorageEntity>> GetAllEntitiesAsync()
		{
			var table = GetTable();
			return await GetAllEntitiesAsync(table);
		}

		private async Task<IEnumerable<StorageEntity>> GetAllEntitiesAsync(CloudTable table)
		{
			List<StorageEntity> result = new List<StorageEntity>();

			// thanks to https://stackoverflow.com/a/48227035/2023653			
			var criteria = TableQuery.GenerateFilterCondition(nameof(StorageEntity.PartitionKey), QueryComparisons.Equal, _partitionKey);
			var query = new TableQuery<StorageEntity>().Where(criteria);
			TableContinuationToken continuationToken = null;
			do
			{
				var results = await table.ExecuteQuerySegmentedAsync(query, continuationToken);
				foreach (var item in results) result.Add(item);
				continuationToken = results.ContinuationToken;
			}
			while (continuationToken != null);

			return result;
		}
	}
}