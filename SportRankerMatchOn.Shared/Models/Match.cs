using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SportRankerMatchOn.Shared
{
	public class Match : BaseModel
	{
		[JsonProperty("member-ids")]
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

		[JsonProperty("start-date")]
		public DateTime StartDate
		{
			get;
			set;
		}

		[JsonProperty("end-date")]
		public DateTime EndDate
		{
			get;
			set;
		}

		[JsonProperty("game-results")]
		public List<GameResult> GameResults
		{
			get;
			set;
		}

		[JsonProperty("league-id")]
		public string LeagueId
		{
			get;
			set;
		}
	}
}