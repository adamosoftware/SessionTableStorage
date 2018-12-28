using SessionTableStorage.Library.Interfaces;
using System;
using System.Threading.Tasks;

namespace SessionTableStorage.Library
{
	/// <summary>
	/// Use this to store data that can be cached for a few minutes or hours before having to be queried again
	/// </summary>
	public abstract class CacheableStorageBase : SessionStorageBase
	{
		public CacheableStorageBase(string partitionKey = "app-data-cacheable") : base(partitionKey)
		{
		}

		public new async Task<T> GetAsync<T>(string rowKey, T defaultValue = default(T))
		{
			throw new NotImplementedException("To guarantee data freshness, please use the overload that accepts a Func<Task<T>>");
		}

		public async Task<T> GetAsync<T>(string rowKey, Func<Task<T>> update) where T : ITimedCacheable
		{
			T result = default(T);

			try
			{
				result = await base.GetAsync<T>(rowKey);
				if (DateTime.UtcNow.Subtract(result.LastUpdate).TotalMinutes < result.MaxLifetimeMinutes) return result;				
				throw new Exception("Cached data is stale");
			}
			catch
			{
				result = await update.Invoke();
				await SetAsync(rowKey, result);
			}

			return result;
		}

		public async Task SetAsync<T>(string rowKey, T data) where T : ITimedCacheable
		{
			data.LastUpdate = DateTime.UtcNow;
			await SetAsync(rowKey, data);
		}
	}
}