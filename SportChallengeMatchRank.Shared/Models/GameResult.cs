using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SportChallengeMatchRank.Shared
{
	public class GameResult : BaseModel
	{
		[JsonIgnore]
		public Challenge Challenge
		{
			get
			{
				return ChallengeId == null ? null : DataManager.Instance.Challenges.Get(ChallengeId);
			}
		}

		string _challengeId;
		public const string ChallengeIdPropertyName = "ChallengeId";

		public string ChallengeId
		{
			get
			{
				return _challengeId;
			}
			set
			{
				SetProperty(ref _challengeId, value, ChallengeIdPropertyName);
			}
		}

		int challengerScore;
		public const string ChallengerScorePropertyName = "ChallengerScore";

		public int ChallengerScore
		{
			get
			{
				return challengerScore;
			}
			set
			{
				SetProperty(ref challengerScore, value, ChallengerScorePropertyName);
			}
		}

		int challengeeScore;
		public const string ChallengeeScorePropertyName = "ChallengeeScore";

		public int ChallengeeScore
		{
			get
			{
				return challengeeScore;
			}
			set
			{
				SetProperty(ref challengeeScore, value, ChallengeeScorePropertyName);
			}
		}

		int? index;
		public const string IndexPropertyName = "Index";

		public int? Index
		{
			get
			{
				return index;
			}
			set
			{
				SetProperty(ref index, value, IndexPropertyName);
			}
		}
	}
}