﻿using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;

namespace SessionTableStorage.Library
{
	/// <summary>
	/// Used internally to mediate between the end-user's data and the backing table storage object
	/// </summary>
	public class StorageEntity : TableEntity
	{
		public StorageEntity()
		{
		}		

		public StorageEntity(string partitionKey, string rowKey, object data, Func<object, string> serializer = null)
		{
			PartitionKey = partitionKey;
			RowKey = rowKey;
			Json = serializer?.Invoke(data) ?? JsonConvert.SerializeObject(data);
		}

		public string Json { get; set; }
	}
}