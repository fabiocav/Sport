using System;
using System.Windows.Input;
using Xamarin.Forms;
using System.Threading.Tasks;

[assembly: Dependency(typeof(SportChallengeMatchRank.Shared.MembershipDetailsViewModel))]

namespace SportChallengeMatchRank.Shared
{
	public class MembershipDetailsViewModel : BaseViewModel
	{
		string _membershipId;

		public string MembershipId
		{
			get
			{
				return _membershipId;
			}
			set
			{
				_membershipId = value;
				SetPropertyChanged("Membership");
			}
		}

		public bool CanDeleteMembership
		{
			get
			{
				return App.CurrentAthlete.IsAdmin && Membership.Id != null;
			}
		}

		public Membership Membership
		{
			get
			{
				return MembershipId == null ? null : DataManager.Instance.Memberships.Get(MembershipId);
			}
		}

		public bool CanRevokeChallenge
		{
			get
			{
				var challenge = Membership.GetExistingOngoingChallengeWithAthlete(App.CurrentAthlete);
				return challenge != null && challenge.ChallengerAthleteId == App.CurrentAthlete.Id && challenge.ChallengeeAthleteId == Membership.AthleteId;
			}
		}

		public bool CanChallenge
		{
			get
			{
				return Membership.CanChallengeAthlete(App.CurrentAthlete);
			}
		}

		public string JoinDescription
		{
			get
			{
				return "joined {0} on {1}".Fmt(Membership.League.Name, Membership.DateCreated.Value.ToString("M"));
			}
		}

		public string RankDescription
		{
			get
			{
				var dayCount = Math.Round(DateTime.UtcNow.Subtract(Membership.LastRankChangeDate).TotalDays);
				return "ranked {0} for {1} day{2}".Fmt(Membership.CurrentRankDisplay.ToOrdinal(), dayCount, dayCount == 1 ? "" : "s");
			}
		}

		async public Task RevokeExistingChallenge(Membership membership)
		{
			var challenge = membership.GetExistingOngoingChallengeWithAthlete(App.CurrentAthlete);
			await RunSafe(InternetService.Instance.RevokeChallenge(challenge.Id));
			App.CurrentAthlete.RefreshChallenges();
			Membership.Athlete.RefreshChallenges();

			NotifyPropertiesChanged();
		}

		async public Task<Challenge> ChallengeAthlete(Membership membership)
		{
			var challenge = new Challenge {
				ChallengerAthleteId = App.CurrentAthlete.Id,
				ChallengeeAthleteId = membership.AthleteId,
				ProposedTime = DateTime.Now.AddHours(3),
				LeagueId = membership.LeagueId,
			};

			var task = InternetService.Instance.SaveChallenge(challenge);
			await RunSafe(task);

			if(task.IsFaulted)
				return null;

			App.CurrentAthlete.RefreshChallenges();
			Membership.Athlete.RefreshChallenges();
			NotifyPropertiesChanged();

			return challenge;
		}

		public void NotifyPropertiesChanged()
		{
			SetPropertyChanged("CanChallenge");
			SetPropertyChanged("CanRevokeChallenge");
			SetPropertyChanged("Membership");
			SetPropertyChanged("CanDeleteMembership");
		}

		public ICommand SaveCommand
		{
			get
			{
				return new Command(async(param) =>
					await SaveMembership());
			}
		}

		async public Task SaveMembership()
		{
			await RunSafe(InternetService.Instance.SaveMembership(Membership));
		}

		async public Task DeleteMembership()
		{
			await RunSafe(InternetService.Instance.DeleteMembership(Membership.Id));
		}
	}
}