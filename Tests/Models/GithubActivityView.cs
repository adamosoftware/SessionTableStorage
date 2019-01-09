using SessionTableStorage.Library.Enums;
using SessionTableStorage.Library.Interfaces;
using System;
using System.Collections.Generic;

namespace Tests.Models
{
	public class GithubActivityView : ITimedCacheable
	{
		public IEnumerable<GithubActivityFlattened> Activities { get; set; }

		public DateTime LastUpdate { get; set; }
		public int MaxLifetimeMinutes => 15;
		public RetrievedFrom RetrievedFrom { get; set; }
	}
}