using SessionTableStorage.Library.Enums;
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

		#region hide base methods
		// these methods don't use the proper cache logic, so they should not be used

		public new async Task SetAsync(string rowKey, object data)
		{
			throw new InvalidOperationException("To guarantee correct cache update, please use the overload that accepts a generic T argument.");
		}

		public new void Set(string rowKey, object data)
		{
			throw new InvalidOperationException("To guarantee correct cache update, please use the overload that accepts a generic T argument.");
		}

		public new async Task<T> GetAsync<T>(string rowKey, T defaultValue = default(T))
		{
			throw new InvalidOperationException("To guarantee data freshness, please use the overload that accepts a Func<Task<T>>");
		}

		public new T Get<T>(string rowKey, T defaultValue = default(T))
		{
			throw new InvalidOperationException("To guarantee data freshness, please use the overload that accepts a Func<T>");
		}
		#endregion

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

		public T Get<T>(string rowKey, Func<T> update, T defaultValue = default(T)) where T : ITimedCacheable
		{
			T result = default(T);

			result = base.Get<T>(rowKey, defaultValue);
			if (IsValid(result))
			{
				result.RetrievedFrom = RetrievedFrom.Cache;
				return result;
			}
			else
			{
				result = update.Invoke();				
				Set(rowKey, result);
				result.RetrievedFrom = RetrievedFrom.Live;
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
			if (result == null) return false;
			return DateTime.UtcNow.Subtract(result.LastUpdate).TotalMinutes < result.MaxLifetimeMinutes;
		}
	}
}