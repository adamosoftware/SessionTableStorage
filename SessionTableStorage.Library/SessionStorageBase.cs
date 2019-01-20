using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SessionTableStorage.Library
{
	public abstract class SessionStorageBase
	{
		protected abstract Task<CloudTable> GetTableAsync();

		public SessionStorageBase(string partitionKey)
		{
			if (string.IsNullOrWhiteSpace(partitionKey)) throw new ArgumentException("Can't use null or whitespace partition key with SessionStorageBase");

			PartitionKey = partitionKey;
		}

		public string PartitionKey { get; }

		private TableOperation GetInsertOrReplaceOperation(string rowKey, object data, Func<object, string> serializer = null)
		{
			return TableOperation.InsertOrReplace(new StorageEntity(PartitionKey, rowKey, data, serializer));
		}

		private TableOperation GetQueryOperation(string rowKey)
		{
			return TableOperation.Retrieve<StorageEntity>(PartitionKey, rowKey);
		}

		/// <summary>
		/// Inserts/replaces the rowKey data
		/// </summary>
		public async Task SetAsync(string rowKey, object data, Func<object, string> serializer = null)
		{
			var insertOrReplace = GetInsertOrReplaceOperation(rowKey, data, serializer);
			var table = await GetTableAsync();
			await table.ExecuteAsync(insertOrReplace);
		}		

		/// <summary>
		/// Queries the cloud table and returns the TableResult object
		/// </summary>
		public async Task<TableResult> QueryTableAsync(string rowKey)
		{
			TableOperation query = GetQueryOperation(rowKey);
			var table = await GetTableAsync();
			return await table.ExecuteAsync(query);
		}

		/// <summary>
		/// Queries the cloud table and returns the T-typed data.		
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
				return defaultValue;
			}
		}

		public async Task DeleteAsync(string rowKey)
		{
			var table = await GetTableAsync();
			var result = await QueryTableAsync(rowKey);
			if (result?.Result != null)
			{				
				var delete = TableOperation.Delete((StorageEntity)result.Result);
				await table.ExecuteAsync(delete);
			}
		}

		/// <summary>
		/// Use like Session.Abandon
		/// </summary>		
		public async Task ClearAsync()
		{
			var table = await GetTableAsync();
			var entities = await GetAllEntitiesAsync(table);
			foreach (var e in entities)
			{
				var delete = TableOperation.Delete(e);
				await table.ExecuteAsync(delete);
			}
		}

		public async Task<IEnumerable<StorageEntity>> GetAllEntitiesAsync()
		{
			var table = await GetTableAsync();
			return await GetAllEntitiesAsync(table);
		}

		private async Task<IEnumerable<StorageEntity>> GetAllEntitiesAsync(CloudTable table)
		{
			List<StorageEntity> result = new List<StorageEntity>();

			// thanks to https://stackoverflow.com/a/48227035/2023653
			var criteria = TableQuery.GenerateFilterCondition(nameof(StorageEntity.PartitionKey), QueryComparisons.Equal, PartitionKey);
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