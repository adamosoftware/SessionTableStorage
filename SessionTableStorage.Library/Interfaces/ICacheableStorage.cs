using System;
using System.Threading.Tasks;
using SessionTableStorage.Library.Interfaces;

namespace SessionTableStorage.Library
{
	public interface ICacheableStorage
	{
		Task<T> GetAsync<T>(string rowKey, Func<Task<T>> query, T defaultValue = default(T)) where T : ICacheable;
		Task InvalidateAsync<T>(string rowKey) where T : ICacheable;
		Task SetAsync<T>(string rowKey, T data, Func<object, string> serizlizer = null) where T : ICacheable;
	}
}