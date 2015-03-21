using Microsoft.WindowsAzure.Mobile.Service;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace SportRankerMatchOn.Shared
{
	public partial class League : EntityData
	{
		[JsonProperty("name")]
		public string Name
		{
			get;
			set;
		}

		[JsonProperty("season")]
		public int Season
		{
			get;
			set;
		}

		[JsonProperty("sport")]
		public string Sport
		{
			get;
			set;
		}

		[JsonProperty("is-accepting-new-members")]
		public bool IsAcceptingMembers
		{
			get;
			set;
		}

		[JsonProperty("is-enabled")]
		public bool IsEnabled
		{
			get;
			set;
		}

		[JsonProperty("member-ids")]
		public List<string> MemberIds
		{
			get;
			set;
		}

		[JsonProperty("member-ids-string")]
		public string MemberIdsString
		{
			get;
			set;
		}
	}
}