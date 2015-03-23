using Microsoft.WindowsAzure.Mobile.Service;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace SportRankerMatchOn
{
	public partial class LeagueDto : LeagueBase
	{
		public List<string> MembershipIds
		{
			get;
			set;
		}
	}
}