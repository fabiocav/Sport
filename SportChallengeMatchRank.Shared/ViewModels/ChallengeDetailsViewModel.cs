using System;
using Xamarin.Forms;
using System.Threading.Tasks;

[assembly: Dependency(typeof(SportChallengeMatchRank.Shared.ChallengeDetailsViewModel))]

namespace SportChallengeMatchRank.Shared
{
	public class ChallengeDetailsViewModel : BaseViewModel
	{
		public ChallengeDetailsViewModel()
		{
			Challenge = new Challenge();
		}

		public ChallengeDetailsViewModel(Challenge challenge)
		{
			Challenge = challenge;
		}

		Challenge challenge;
		public const string ChallengePropertyName = "Challenge";

		public Challenge Challenge
		{
			get
			{
				return challenge;
			}
			set
			{
				SetProperty(ref challenge, value, ChallengePropertyName);
			}
		}

		public bool CanAccept
		{
			get
			{
				return Challenge.ChallengeeAthleteId == App.CurrentAthlete.Id && !challenge.IsAccepted && !Challenge.IsCompleted;
			}
		}

		public bool CanDecline
		{
			get
			{
				return Challenge.ChallengeeAthleteId == App.CurrentAthlete.Id && !Challenge.IsCompleted;
			}
		}

		public bool CanRevoke
		{
			get
			{
				return Challenge.ChallengerAthleteId == App.CurrentAthlete.Id && !Challenge.IsCompleted;
			}
		}

		public bool CanPostMatchResults
		{
			get
			{
				return Challenge.IsAccepted && !Challenge.IsCompleted;
			}
		}

		async public Task GetMatchResults(bool forceRefresh = false)
		{
			if(!forceRefresh && Challenge.MatchResult.Count > 0)
				return;

			var results = await AzureService.Instance.Client.GetTable<GameResult>().Where(r => r.ChallengeId == Challenge.Id).OrderBy(r => r.Index).ToListAsync();

			Challenge.MatchResult.Clear();
			results.ForEach(Challenge.MatchResult.Add);
			OnPropertyChanged("Challenge");
		}

		async public Task AcceptChallenge()
		{
			using(new Busy(this))
			{
				try
				{
					await AzureService.Instance.AcceptChallenge(Challenge);
					NotifyPropertiesChanged();
				}
				catch(Exception e)
				{
					Console.WriteLine(e);
				}
			}
		}

		async public Task DeclineChallenge()
		{
			using(new Busy(this))
			{
				try
				{
					await AzureService.Instance.DeclineChallenge(Challenge.Id);
					NotifyPropertiesChanged();
				}
				catch(Exception e)
				{
					Console.WriteLine(e);
				}
			}
		}

		public void NotifyPropertiesChanged()
		{
			OnPropertyChanged("CanAccept");
			OnPropertyChanged("CanDecline");
			OnPropertyChanged("CanRevoke");
			OnPropertyChanged("CanPostMatchResults");
			OnPropertyChanged("Challenge");
		}
	}
}