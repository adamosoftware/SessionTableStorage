using SessionTableStorage.Library.Enums;
using SessionTableStorage.Library.Interfaces;
using System;
using System.Threading.Tasks;

namespace SessionTableStorage.Library
{
	public abstract class CacheableStorage : SessionStorageBase, ICacheableStorage
	{
		public CacheableStorage(string partitionKey) : base(partitionKey)
		{
		}

		public async Task<T> GetAsync<T>(string rowKey, Func<Task<T>> query, T defaultValue = default(T)) where T : ICacheable
		{
			T result = await GetAsync(rowKey, defaultValue);
			if (result?.IsValid ?? false)
			{
				result.RetrievedFrom = RetrievedFrom.Cache;
				return result;
			}

			result = await query.Invoke();
			await SetAsync(rowKey, result);

			result.RetrievedFrom = RetrievedFrom.Live;
			return result;
		}

		public async Task SetAsync<T>(string rowKey, T data, Func<object, string> serizlizer = null) where T : ICacheable
		{
			data.IsValid = true;
			await base.SetAsync(rowKey, data, serizlizer);
		}

		public async Task InvalidateAsync<T>(string rowKey) where T : ICacheable
		{
			T result = await base.GetAsync<T>(rowKey);
			if (result.Equals(default(T))) return;

			result.IsValid = false;
			await base.SetAsync(rowKey, result);
		}
	}
}