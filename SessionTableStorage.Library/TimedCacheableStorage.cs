using SessionTableStorage.Library.Enums;
using SessionTableStorage.Library.Interfaces;
using System;
using System.Threading.Tasks;

namespace SessionTableStorage.Library
{
	/// <summary>
	/// Use this to store data that can be cached for a fixed time before having to be queried again
	/// </summary>
	public abstract class TimedCacheableStorage : SessionStorageBase, ITimedCacheableStorage
	{
		public TimedCacheableStorage(string partitionKey = "app-timed-cacheable") : base(partitionKey)
		{
		}

		public async Task<T> GetAsync<T>(string rowKey, Func<Task<T>> update, T defaultValue = default(T)) where T : ITimedCacheable
		{
			T result = default(T);

			result = await base.GetAsync(rowKey, defaultValue);

			if (IsValid(result))
			{
				result.RetrievedFrom = RetrievedFrom.Cache;
				return result;
			}
			else
			{
				result = await update.Invoke();				
				await SetAsync(rowKey, result);
				result.RetrievedFrom = RetrievedFrom.Live;
			}			

			return result;
		}

		public async Task SetAsync<T>(string rowKey, T data, Func<object, string> serializer = null) where T : ITimedCacheable
		{
			data.LastUpdate = DateTime.UtcNow;
			await base.SetAsync(rowKey, data, serializer);
		}

		private static bool IsValid<T>(T result) where T : ITimedCacheable
		{
			if (result == null) return false;
			return DateTime.UtcNow.Subtract(result.LastUpdate).TotalMinutes < result.MaxLifetimeMinutes;
		}
	}
}