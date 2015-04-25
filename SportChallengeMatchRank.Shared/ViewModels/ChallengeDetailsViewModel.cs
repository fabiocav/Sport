using System;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;

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

		public Challenge Challenge
		{
			get
			{
				return challenge;
			}
			set
			{
				SetPropertyChanged(ref challenge, value);
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
				return Challenge.ChallengeeAthleteId == App.CurrentAthlete.Id && !Challenge.IsCompleted && !Challenge.IsAccepted;
			}
		}

		public bool CanDeclineAfterAccept
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

			var task = new Task<List<GameResult>>(() => InternetService.Instance.Client.GetTable<GameResult>().Where(r => r.ChallengeId == Challenge.Id).OrderBy(r => r.Index).ToListAsync().Result);
			await RunSafe(task);

			if(task.IsFaulted)
				return;

			var results = task.Result;

			Challenge.MatchResult.Clear();
			results.ForEach(Challenge.MatchResult.Add);
			SetPropertyChanged("Challenge");
		}

		async public Task<bool> AcceptChallenge()
		{
			var task = InternetService.Instance.AcceptChallenge(Challenge);
			await RunSafe(task);
			NotifyPropertiesChanged();
			return !task.IsFaulted;
		}

		async public Task<bool> DeclineChallenge()
		{
			var task = InternetService.Instance.DeclineChallenge(Challenge.Id);
			await RunSafe(task);
			App.CurrentAthlete.RefreshChallenges();
			NotifyPropertiesChanged();
			return !task.IsFaulted;
		}

		public void NotifyPropertiesChanged()
		{
			SetPropertyChanged("CanAccept");
			SetPropertyChanged("CanDecline");
			SetPropertyChanged("CanRevoke");
			SetPropertyChanged("CanPostMatchResults");
			SetPropertyChanged("Challenge");
		}
	}
}