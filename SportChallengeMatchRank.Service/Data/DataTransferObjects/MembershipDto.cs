using Microsoft.WindowsAzure.Mobile.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SportChallengeMatchRank
{
	public partial class MembershipDto : MembershipBase
	{
		public DateTimeOffset? DateCreated
		{
			get;
			set;
		}

		public List<Challenge> OngoingChallenges
		{
			get;
			set;
		}
	}
}