using System;
using System.Threading.Tasks;
using SessionTableStorage.Library.Interfaces;

namespace SessionTableStorage.Library
{
	public interface ITimedCacheableStorage
	{				
		Task<T> GetAsync<T>(string rowKey, Func<Task<T>> update, T defaultValue = default(T)) where T : ITimedCacheable;				
		Task SetAsync<T>(string rowKey, T data, Func<object, string> serializer = null) where T : ITimedCacheable;
	}
}