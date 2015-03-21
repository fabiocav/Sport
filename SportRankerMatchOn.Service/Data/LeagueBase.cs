using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Tables;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace SportRankerMatchOn
{
	public partial class LeagueBase : EntityData
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

		[JsonProperty("is_accepting_new_members")]
		public bool IsAcceptingMembers
		{
			get;
			set;
		}

		[JsonProperty("is_enabled")]
		public bool IsEnabled
		{
			get;
			set;
		}
	}
}