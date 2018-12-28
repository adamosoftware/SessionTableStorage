using Octokit;
using System;
using System.Linq;

namespace Tests.Models
{
	public class GithubActivityFlattened
	{
		public GithubActivityFlattened()
		{
		}

		public GithubActivityFlattened(Activity e)
		{
			CreatedAt = e.CreatedAt.Date;
			EventType = e.Type;
			RepositoryName = e.Repo.Name.Split('/').Last();
			RepositoryUrl = $"https://github.com/{e.Repo.Name}";

			var push = e.Payload as PushEventPayload;
			if (push != null)
			{				
				Commits = push.Commits.Select(c => new Commit() { Message = c.Message, Url = c.Url }).ToArray();
			}
		}

		public DateTime CreatedAt { get; set; }
		public string EventType { get; set; }
		public string RepositoryName { get; set; }
		public string RepositoryUrl { get; set; }
		public Commit[] Commits { get; set; }

		public class Commit
		{			
			public string Message { get; set; }
			public string Url { get; set; }
		}
	}
}