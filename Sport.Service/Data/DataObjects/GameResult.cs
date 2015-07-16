using Microsoft.WindowsAzure.Mobile.Service;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace SportChallengeMatchRank
{
	public partial class GameResult : GameResultBase
	{
		public Challenge Challenge
		{
			get;
			set;
		}
	}
}