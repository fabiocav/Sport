using Microsoft.WindowsAzure.Mobile.Service;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace SportRankerMatchOn
{
	public partial class League : LeagueBase
	{
		public League()
		{
			Members = new HashSet<Member>();
		}

		public ICollection<Member> Members
		{
			get;
			set;
		}
	}
}