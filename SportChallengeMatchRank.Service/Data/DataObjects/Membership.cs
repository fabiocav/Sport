using Microsoft.WindowsAzure.Mobile.Service;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Mobile.Service.Tables;
using Newtonsoft.Json;

namespace SportChallengeMatchRank
{
	public partial class Membership : MembershipBase
	{
		public League League
		{
			get;
			set;
		}

		public Athlete Athlete
		{
			get;
			set;
		}
	}
}