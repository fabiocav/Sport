using Microsoft.WindowsAzure.Mobile.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SportChallengeMatchRank
{
	public partial class ChallengeDto : ChallengeBase
	{
		public DateTimeOffset? DateCreated
		{
			get;
			set;
		}
	}
}