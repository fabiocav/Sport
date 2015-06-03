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

		#region Properties

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
				NotifyPropertiesChanged();
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
				return Challenge.ChallengeeAthleteId == App.CurrentAthlete.Id && Challenge.IsAccepted && !Challenge.IsCompleted;
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
				return Challenge.IsAccepted && !Challenge.IsCompleted &&
				(Challenge.ChallengeeAthleteId == App.CurrentAthlete.Id || Challenge.ChallengerAthleteId == App.CurrentAthlete.Id);
			}
		}

		public Athlete Opponent
		{
			get
			{
				return Challenge.ChallengeeAthleteId != App.CurrentAthlete.Id ? Challenge.ChallengeeAthlete : Challenge.ChallengerAthlete;
			}
		}

		public string ChallengeStatus
		{
			get
			{
				string status = null;

				if(CanAccept)
					return "it's time to a make a decision... green pill or red pill?";

				if(CanPostMatchResults)
					return "this is where you'll reflect upon your victorious match result score... but you'll have to post some results first using this here button";

				if(CanRevoke)
					return "...just waiting for your opponent to accept your challenge or coward out like shameless slimeball";

				if(!Challenge.IsCompleted)
					return "the results of this might duel have not yet been posted... check back soon for some tasty scores!";
				
				return status;
			}
		}


		#endregion

		async public Task GetMatchResults(bool forceRefresh = false)
		{
			if(!forceRefresh && Challenge.MatchResult.Count > 0)
				return;

			var task = new Task<List<GameResult>>(() => AzureService.Instance.Client.GetTable<GameResult>().Where(r => r.ChallengeId == Challenge.Id).OrderBy(r => r.Index).ToListAsync().Result);
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
			var task = AzureService.Instance.AcceptChallenge(Challenge);
			await RunSafe(task);
			NotifyPropertiesChanged();
			return !task.IsFaulted;
		}

		async public Task<bool> DeclineChallenge()
		{
			Task task = null;
			if(App.CurrentAthlete.Id == Challenge.ChallengerAthleteId)
			{
				task = AzureService.Instance.RevokeChallenge(Challenge.Id);
			}
			else if(App.CurrentAthlete.Id == Challenge.ChallengeeAthleteId)
			{
				task = AzureService.Instance.DeclineChallenge(Challenge.Id);
			}

			await RunSafe(task);

			Challenge.ChallengeeAthlete.RefreshChallenges();
			Challenge.ChallengerAthlete.RefreshChallenges();

			return !task.IsFaulted;
		}

		async public Task RefreshChallenge()
		{
			using(new Busy(this))
			{
				var task = AzureService.Instance.GetChallengeById(Challenge.Id, true);
				await RunSafe(task);

				if(task.IsFaulted)
					return;

				Challenge = task.Result;
			}
		}

		public void NotifyPropertiesChanged()
		{
			SetPropertyChanged("CanAccept");
			SetPropertyChanged("CanDecline");
			SetPropertyChanged("CanDeclineAfterAccept");
			SetPropertyChanged("CanRevoke");
			SetPropertyChanged("CanPostMatchResults");
			SetPropertyChanged("Challenge");
			SetPropertyChanged("ChallengeStatus");
			SetPropertyChanged("Opponent");
		}
	}
}