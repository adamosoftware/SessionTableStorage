using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace SessionTableStorage.Library
{
	public interface ISessionStorageBase
	{
		string PartitionKey { get; }

		Task ClearAsync();
		Task DeleteAsync(string rowKey);
		Task<IEnumerable<StorageEntity>> GetAllEntitiesAsync();
		Task<T> GetAsync<T>(string rowKey, T defaultValue = default(T));
		Task<TableResult> QueryTableAsync(string rowKey);
		Task SetAsync(string rowKey, object data, Func<object, string> serializer = null);
	}
}