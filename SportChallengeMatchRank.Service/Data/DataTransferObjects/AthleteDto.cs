﻿using Microsoft.WindowsAzure.Mobile.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SportChallengeMatchRank
{
	public partial class AthleteDto : AthleteBase
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

		public List<string> IncomingChallengeIds
		{
			get;
			set;
		}

		public List<string> OutgoingChallengeIds
		{
			get;
			set;
		}
	}
}