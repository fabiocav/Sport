using System;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Linq;

[assembly: Dependency(typeof(SportChallengeMatchRank.Shared.MatchResultFormViewModel))]

namespace SportChallengeMatchRank.Shared
{
	public class MatchResultFormViewModel : BaseViewModel
	{
		string _challengeId;

		public string ChallengeId
		{
			get
			{
				return _challengeId;
			}
			set
			{
				_challengeId = value;
				SetPropertyChanged("Challenge");
			}
		}

		public Challenge Challenge
		{
			get
			{
				return ChallengeId == null ? null : DataManager.Instance.Challenges.Get(ChallengeId);
			}
		}

		async public Task PostMatchResults()
		{
			foreach(var gr in Challenge.MatchResult.ToList())
			{
				if(gr.ChallengeeScore == null || gr.ChallengerScore == null)
					Challenge.MatchResult.Remove(gr);
			}

			await RunSafe(AzureService.Instance.PostMatchResults(Challenge));
		}
	}
}