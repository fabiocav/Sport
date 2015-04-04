using System;
using Xamarin.Forms;
using System.Threading.Tasks;

[assembly: Dependency(typeof(SportChallengeMatchRank.Shared.MatchResultsFormViewModel))]

namespace SportChallengeMatchRank.Shared
{
	public class MatchResultsFormViewModel : BaseViewModel
	{
		public MatchResultsFormViewModel()
		{
			Challenge = new Challenge();
		}

		public MatchResultsFormViewModel(Challenge challenge)
		{
			Challenge = challenge;
		}

		Challenge _challenge;
		public const string ChallengePropertyName = "Challenge";

		public Challenge Challenge
		{
			get
			{
				return _challenge;
			}
			set
			{
				SetProperty(ref _challenge, value, ChallengePropertyName);
			}
		}

		async public Task PostMatchResults()
		{
			await AzureService.Instance.PostMatchResults(Challenge);
		}
	}
}