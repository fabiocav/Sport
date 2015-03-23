using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SportChallengeMatchRank.Shared
{
	public class GameResult : BaseModel
	{
		int _winningScore;
		public const string WinningScorePropertyName = "WinningScore";

		public int WinningScore
		{
			get
			{
				return _winningScore;
			}
			set
			{
				SetProperty(ref _winningScore, value, WinningScorePropertyName);
			}
		}

		int _losingScore;
		public const string LosingScorePropertyName = "LosingScore";

		public int LosingScore
		{
			get
			{
				return _losingScore;
			}
			set
			{
				SetProperty(ref _losingScore, value, LosingScorePropertyName);
			}
		}

		List<string> _victoriousMemberIDs;
		public const string VictoriousMemberIDsPropertyName = "VictoriousMemberIDs";

		public List<string> VictoriousMemberIDs
		{
			get
			{
				return _victoriousMemberIDs;
			}
			set
			{
				SetProperty(ref _victoriousMemberIDs, value, VictoriousMemberIDsPropertyName);
			}
		}

	}
}