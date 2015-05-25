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

		public string ValidateMatchResults()
		{
			var challengeeWins = 0;
			var challengerWins = 0;
			foreach(var g in Challenge.MatchResult)
			{
				if(!g.ChallengeeScore.HasValue && !g.ChallengerScore.HasValue)
					continue;

				if((g.ChallengeeScore.HasValue && !g.ChallengerScore.HasValue) || (!g.ChallengeeScore.HasValue && g.ChallengerScore.HasValue))
					return "Please ensure both players have valid scores.";
				
				if(g.ChallengeeScore > g.ChallengerScore)
				{
					challengeeWins++;
				}
				else if(g.ChallengerScore > g.ChallengeeScore)
				{
					challengerWins++;
				}
				else
				{
					return "Please ensure there are no tie scores.";
				}
			}

			var minWins = Math.Ceiling(Challenge.League.MatchGameCount / 2f);

			if(challengeeWins == challengerWins || (challengeeWins < minWins && challengerWins < minWins))
				return "Please ensure there is a clear victor.";
		
			return null;
		}
	}
}