using Microsoft.WindowsAzure.Mobile.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SportRankerMatchOn
{
	public partial class LeagueDto : LeagueBase
	{
		public DateTimeOffset? DateCreated
		{
			get;
			set;
		}

		public List<string> MembershipIds
		{
			get;
			set;
		}
	}
}