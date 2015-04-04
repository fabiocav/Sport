using Microsoft.WindowsAzure.Mobile.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SportChallengeMatchRank
{
	public partial class ChallengeDto : ChallengeBase
	{
		public ChallengeDto() : base()
		{
			GameResults = new List<GameResultDto>();
		}

		public DateTimeOffset? DateCreated
		{
			get;
			set;
		}

		public List<GameResultDto> GameResults
		{
			get;
			set;
		}
	}
}