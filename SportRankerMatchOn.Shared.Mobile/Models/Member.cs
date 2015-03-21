using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SportRankerMatchOn.Shared
{
	public class Member : BaseModel
	{
		[JsonProperty("athlete_id")]
		public string AthleteId
		{
			get;
			set;
		}

		[JsonIgnore]
		public Athlete Athlete
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

		[JsonIgnore]
		public League League
		{
			get;
			set;
		}

		[JsonProperty("join_date")]
		public DateTime JoinDate
		{
			get;
			set;
		}

		[JsonProperty("current_rank")]
		public int CurrentRank
		{
			get;
			set;
		}
	}
}