using Microsoft.WindowsAzure.Mobile.Service;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace SportChallengeMatchRank
{
	public partial class Challenge : ChallengeBase
	{
		public Challenge() : base()
		{
			GameResults = new List<GameResult>();
		}

		public League League
		{
			get;
			set;
		}

		public Athlete ChallengerAthlete
		{
			get;
			set;
		}

		public Athlete ChallengeeAthlete
		{
			get;
			set;
		}

		public List<GameResult> GameResults
		{
			get;
			set;
		}
	}
}