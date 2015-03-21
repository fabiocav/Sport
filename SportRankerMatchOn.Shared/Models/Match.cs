using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SportRankerMatchOn.Shared
{
	public class Match : BaseModel
	{
		[JsonProperty("member_ids")]
		public List<string> MemberIds
		{
			get;
			set;
		}

		[JsonIgnore]
		public List<Member> Members
		{
			get;
			set;
		}

		[JsonProperty("start_date")]
		public DateTime StartDate
		{
			get;
			set;
		}

		[JsonProperty("end_date")]
		public DateTime EndDate
		{
			get;
			set;
		}

		[JsonProperty("game_results")]
		public List<GameResult> GameResults
		{
			get;
			set;
		}

		[JsonProperty("league_id")]
		public string LeagueId
		{
			get;
			set;
		}
	}
}