using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SportRankerMatchOn.Shared
{
	public class GameResult
	{
		[JsonProperty("winning-score")]
		public int WinningScore
		{
			get;
			set;
		}

		[JsonProperty("losing-score")]
		public int LosingScore
		{
			get;
			set;
		}

		[JsonProperty("victorious-member-ids")]
		public List<string> VictoriousMemberIDs
		{
			get;
			set;
		}
	}
}