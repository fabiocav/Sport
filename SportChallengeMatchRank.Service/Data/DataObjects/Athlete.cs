using Microsoft.WindowsAzure.Mobile.Service;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace SportChallengeMatchRank
{
	public partial class Athlete : AthleteBase
	{
		public Athlete()
		{
			Memberships = new HashSet<Membership>();
			IncomingChallenges = new HashSet<Challenge>();
			OutgoingChallenges = new HashSet<Challenge>();
		}

		public ICollection<Challenge> IncomingChallenges
		{
			get;
			set;
		}

		public ICollection<Challenge> OutgoingChallenges
		{
			get;
			set;
		}

		public ICollection<Membership> Memberships
		{
			get;
			set;
		}
	}
}