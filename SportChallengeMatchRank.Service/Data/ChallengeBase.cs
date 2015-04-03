using Microsoft.WindowsAzure.Mobile.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SportChallengeMatchRank
{
	public partial class ChallengeBase : EntityData
	{
		public string LeagueId
		{
			get;
			set;
		}

		public string ChallengerAthleteId
		{
			get;
			set;
		}

		public string ChallengeeAthleteId
		{
			get;
			set;
		}

		public DateTimeOffset? ProposedTime
		{
			get;
			set;
		}

		public DateTimeOffset? DateAccepted
		{
			get;
			set;
		}

		public bool IsAccepted
		{
			get;
			set;
		}

		public string CustomMessage
		{
			get;
			set;
		}
	}
}