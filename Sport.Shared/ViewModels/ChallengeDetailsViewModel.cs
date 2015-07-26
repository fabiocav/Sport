using System;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;

[assembly: Dependency(typeof(Sport.Shared.ChallengeDetailsViewModel))]

namespace Sport.Shared
{
	public class ChallengeDetailsViewModel : ChallengeViewModel
	{
		public ChallengeDetailsViewModel() : base(null)
		{
			Challenge = new Challenge();
		}

		public ChallengeDetailsViewModel(Challenge challenge) : base(challenge)
		{
			Challenge = challenge;
		}

		#region Properties

		public bool CanAccept
		{
			get
			{
				return Challenge != null && Challenge.ChallengeeAthleteId == App.CurrentAthlete.Id && !_challenge.IsAccepted && !Challenge.IsCompleted;
			}
		}

		public bool CanDecline
		{
			get
			{
				return CanAccept;
			}
		}

		public bool CanDeclineAfterAccept
		{
			get
			{
				return Challenge != null && Challenge.ChallengeeAthleteId == App.CurrentAthlete.Id && Challenge.IsAccepted && !Challenge.IsCompleted;
			}
		}

		public bool CanRevoke
		{
			get
			{
				return Challenge != null && Challenge.ChallengerAthleteId == App.CurrentAthlete.Id && !Challenge.IsCompleted;
			}
		}

		public bool CanPostMatchResults
		{
			get
			{
				return Challenge != null && Challenge.IsAccepted && !Challenge.IsCompleted && Challenge.InvolvesAthlete(App.CurrentAthlete.Id);
			}
		}

		public bool AwaitingDecision
		{
			get
			{
				return Challenge != null && !CanAccept && !CanPostMatchResults && Challenge.InvolvesAthlete(App.CurrentAthlete.Id) && !Challenge.IsCompleted;
			}
		}

		public Athlete Opponent
		{
			get
			{
				return Challenge != null && Challenge.ChallengeeAthleteId != App.CurrentAthlete.Id ? Challenge.ChallengeeAthlete : Challenge.ChallengerAthlete;
			}
		}

		public string ChallengeStatus
		{
			get
			{
				if(Challenge == null)
					return null;
				
				string status = null;

				if(CanAccept)
					return "do you accept this honorable duel?";

				if(CanPostMatchResults)
					return "this is where you'll reflect upon your victorious match score... but you'll have to post some results first";

				if(CanRevoke)
					return "...just waiting for your opponent to accept your challenge or coward out like a shameless hunk of slime";

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
			MessagingCenter.Send<App>(App.Current, "ChallengesUpdated");
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

			Challenge.League.RefreshChallenges();
			MessagingCenter.Send<App>(App.Current, "ChallengesUpdated");
			return !task.IsFaulted;
		}

		async public Task NudgeAthlete()
		{
			using(new Busy(this))
			{
				var task = AzureService.Instance.NagAthlete(Challenge.Id);
				await RunSafe(task);

				if(task.IsFaulted)
					return;
			}
		}

		async public Task RefreshChallenge()
		{
			if(Challenge == null)
				return;
			
			using(new Busy(this))
			{
				var task = AzureService.Instance.GetChallengeById(Challenge.Id, true);
				await RunSafe(task);

				if(task.IsFaulted)
				{
					Challenge = null;
					return;
				}

				Challenge = task.Result;
			}
		}

		public override void NotifyPropertiesChanged()
		{
			base.NotifyPropertiesChanged();
			Challenge?.NotifyPropertiesChanged();

			SetPropertyChanged("CanAccept");
			SetPropertyChanged("CanDecline");
			SetPropertyChanged("CanDeclineAfterAccept");
			SetPropertyChanged("CanRevoke");
			SetPropertyChanged("CanPostMatchResults");
			SetPropertyChanged("Challenge");
			SetPropertyChanged("ChallengeStatus");
			SetPropertyChanged("Opponent");
			SetPropertyChanged("AwaitingDecision");
		}
	}
}