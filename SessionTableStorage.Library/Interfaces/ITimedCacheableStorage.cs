using System;
using System.Threading.Tasks;
using SessionTableStorage.Library.Interfaces;

namespace SessionTableStorage.Library
{
	public interface ITimedCacheableStorage
	{
		T Get<T>(string rowKey, T defaultValue = default(T));
		Task<T> GetAsync<T>(string rowKey, T defaultValue = default(T));
		Task<T> GetAsync<T>(string rowKey, Func<Task<T>> update, T defaultValue = default(T)) where T : ITimedCacheable;
		void Set(string rowKey, object data, Func<object, string> serializer = null);
		Task SetAsync(string rowKey, object data, Func<object, string> serializer = null);
		Task SetAsync<T>(string rowKey, T data, Func<object, string> serializer = null) where T : ITimedCacheable;
	}
}