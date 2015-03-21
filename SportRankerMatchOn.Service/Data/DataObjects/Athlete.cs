using Microsoft.WindowsAzure.Mobile.Service;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace SportRankerMatchOn
{
	public partial class Athlete : AthleteBase
	{
		public Athlete()
		{
			LeagueAssociations = new HashSet<Member>();
		}

		public ICollection<Member> LeagueAssociations
		{
			get;
			set;
		}
	}
}