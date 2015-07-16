using Microsoft.WindowsAzure.Mobile.Service;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace SportChallengeMatchRank
{
	public partial class Challenge : ChallengeBase
	{
		public Challenge() : base()
		{
			MatchResult = new List<GameResult>();
		}

		[JsonIgnore]
		public League League
		{
			get;
			set;
		}

		[JsonIgnore]
		public Athlete ChallengerAthlete
		{
			get;
			set;
		}

		[JsonIgnore]
		public Athlete ChallengeeAthlete
		{
			get;
			set;
		}

		public List<GameResult> MatchResult
		{
			get;
			set;
		}
	}
}