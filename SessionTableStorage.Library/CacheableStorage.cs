using SessionTableStorage.Library.Enums;
using SessionTableStorage.Library.Interfaces;
using System;
using System.Threading.Tasks;

namespace SessionTableStorage.Library
{
	public abstract class CacheableStorage : SessionStorageBase
	{
		public CacheableStorage(string partitionKey) : base(partitionKey)
		{
		}

		#region hide base methods
		// these methods don't use the proper cache logic, so they should not be used

		public new async Task SetAsync(string rowKey, object data, Func<object, string> serializer = null)
		{
			throw new InvalidOperationException("To guarantee correct cache update, please use the overload that accepts a generic T argument.");
		}

		public new void Set(string rowKey, object data, Func<object, string> serializer = null)
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

		public async Task<T> GetAsync<T>(string rowKey, Func<Task<T>> query, T defaultValue = default(T)) where T : ICacheable
		{
			T result = await GetAsync(rowKey, defaultValue);
			if (result.IsValid)
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
			T result = await GetAsync<T>(rowKey);
			if (result.Equals(default(T))) return;

			result.IsValid = false;
			await base.SetAsync(rowKey, result);
		}

		public void Invalidate<T>(string rowKey) where T : ICacheable
		{
			T result = Get<T>(rowKey);
			if (result.Equals(default(T))) return;

			result.IsValid = false;
			base.Set(rowKey, result);
		}

		public T Get<T>(string rowKey, Func<T> query, T defaultValue = default(T)) where T : ICacheable
		{
			T result = Get(rowKey, defaultValue);
			if (result.IsValid)
			{
				result.RetrievedFrom = RetrievedFrom.Cache;
				return result;
			}

			result = query.Invoke();
			Set(rowKey, result);

			result.RetrievedFrom = RetrievedFrom.Live;
			return result;
		}

		public void Set<T>(string rowKey, T data, Func<object, string> serializer = null) where T : ICacheable
		{
			data.IsValid = true;
			base.Set(rowKey, data, serializer);
		}
	}
}