using Microsoft.WindowsAzure.Mobile.Service;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace SportRankerMatchOn
{
	public partial class LeagueDto : LeagueBase
	{
		[JsonProperty("member_ids")]
		public List<string> MemberIds
		{
			get;
			set;
		}
	}
}