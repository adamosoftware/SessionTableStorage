using SessionTableStorage.Library.Interfaces;
using System;
using System.Threading.Tasks;

namespace SessionTableStorage.Library
{
	/// <summary>
	/// Use this to store data that can be cached for a fixed time before having to be queried again
	/// </summary>
	public abstract class TimedCacheableStorage : SessionStorageBase
	{
		public TimedCacheableStorage(string partitionKey = "app-timed-cacheable") : base(partitionKey)
		{
		}

		public new async Task<T> GetAsync<T>(string rowKey, T defaultValue = default(T))
		{
			throw new InvalidOperationException("To guarantee data freshness, please use the overload that accepts a Func<Task<T>>");
		}

		public new T Get<T>(string rowKey, T defaultValue = default(T))
		{
			throw new InvalidOperationException("To guarantee data freshness, please use the overload that accepts a Func<T>");
		}

		public async Task<T> GetAsync<T>(string rowKey, Func<Task<T>> update, T defaultValue = default(T)) where T : ITimedCacheable
		{
			T result = default(T);

			result = await base.GetAsync(rowKey, defaultValue);

			if (IsValid(result))
			{
				return result;
			}
			else
			{
				result = await update.Invoke();
				await SetAsync(rowKey, result);
			}			

			return result;
		}

		public T Get<T>(string rowKey, Func<T> update, T defaultValue = default(T)) where T : ITimedCacheable
		{
			T result = default(T);

			result = base.Get<T>(rowKey, defaultValue);
			if (IsValid(result))
			{
				return result;
			}
			else
			{
				result = update.Invoke();
				Set(rowKey, result);
			}
	
			return result;
		}

		public async Task SetAsync<T>(string rowKey, T data) where T : ITimedCacheable
		{
			data.LastUpdate = DateTime.UtcNow;
			await base.SetAsync(rowKey, data);
		}

		public void Set<T>(string rowKey, T data) where T : ITimedCacheable
		{
			data.LastUpdate = DateTime.UtcNow;
			base.Set(rowKey, data);
		}

		private static bool IsValid<T>(T result) where T : ITimedCacheable
		{
			return DateTime.UtcNow.Subtract(result.LastUpdate).TotalMinutes < result.MaxLifetimeMinutes;
		}
	}
}