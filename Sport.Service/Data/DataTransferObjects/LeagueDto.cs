using Microsoft.WindowsAzure.Mobile.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Web.Http;

namespace Sport
{
	public partial class LeagueDto : LeagueBase
	{
		public LeagueDto()
		{
			Memberships = new List<MembershipDto>();
			OngoingChallenges = new List<ChallengeDto>();
		}

		public DateTimeOffset? DateCreated
		{
			get;
			set;
		}

		public virtual List<MembershipDto> Memberships
		{
			get;
			set;
		}

		public virtual List<ChallengeDto> OngoingChallenges
		{
			get;
			set;
		}
	}
}