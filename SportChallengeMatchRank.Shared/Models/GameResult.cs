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

		public string ChallengeId
		{
			get
			{
				return _challengeId;
			}
			set
			{
				ProcPropertyChanged(ref _challengeId, value);
			}
		}

		int? challengerScore;

		public int? ChallengerScore
		{
			get
			{
				return challengerScore;
			}
			set
			{
				ProcPropertyChanged(ref challengerScore, value);
			}
		}

		int? challengeeScore;

		public int? ChallengeeScore
		{
			get
			{
				return challengeeScore;
			}
			set
			{
				ProcPropertyChanged(ref challengeeScore, value);
			}
		}

		int? index;

		public int? Index
		{
			get
			{
				return index;
			}
			set
			{
				ProcPropertyChanged(ref index, value);
			}
		}
	}
}