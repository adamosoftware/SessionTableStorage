﻿using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace SessionTableStorage.Library
{
	public class StorageEntity : TableEntity
	{
		public StorageEntity()
		{
		}

		public StorageEntity(string partitionKey, string rowKey, object data)
		{
			PartitionKey = partitionKey;
			RowKey = rowKey;
			Json = JsonConvert.SerializeObject(data);
		}

		public string Json { get; set; }
	}
}