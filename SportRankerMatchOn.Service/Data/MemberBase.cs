using Microsoft.WindowsAzure.Mobile.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SportRankerMatchOn
{
	public partial class MemberBase : EntityData
	{
		[JsonProperty("athlete_id")]
		public string AthleteId
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

		[JsonProperty("current_rank")]
		public int CurrentRank
		{
			get;
			set;
		}
	}
}