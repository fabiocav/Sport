using Microsoft.WindowsAzure.Mobile.Service;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Sport
{
	public partial class Athlete : AthleteBase
	{
		public Athlete()
		{
			Memberships = new HashSet<Membership>();
			//IncomingChallenges = new HashSet<Challenge>();
			//OutgoingChallenges = new HashSet<Challenge>();
		}

		//[JsonIgnore]
		//public ICollection<Challenge> IncomingChallenges
		//{
		//	get;
		//	set;
		//}

		//[JsonIgnore]
		//public ICollection<Challenge> OutgoingChallenges
		//{
		//	get;
		//	set;
		//}

		[JsonIgnore]
		public ICollection<Membership> Memberships
		{
			get;
			set;
		}
	}
}