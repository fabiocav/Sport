﻿using Microsoft.WindowsAzure.Mobile.Service;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace SportRankerMatchOn
{
	public partial class League : LeagueBase
	{
		public League()
		{
			Memberships = new HashSet<Membership>();
		}

		public ICollection<Membership> Memberships
		{
			get;
			set;
		}
	}
}