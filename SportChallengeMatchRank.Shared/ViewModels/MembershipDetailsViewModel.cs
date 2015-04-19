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
				var challenge = Membership.GetExistingChallengeWithAthlete(App.CurrentAthlete);
				return challenge != null && challenge.ChallengerAthleteId == App.CurrentAthlete.Id;
			}
		}

		public bool CanChallenge
		{
			get
			{
				return Membership.CanChallengeAthlete(App.CurrentAthlete);
			}
		}

		async public Task RevokeExistingChallenge(Membership membership)
		{
			var challenge = membership.GetExistingChallengeWithAthlete(App.CurrentAthlete);
			await RunSafe(InternetService.Instance.DeleteChallenge(challenge.Id));
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

			await RunSafe(InternetService.Instance.SaveChallenge(challenge));
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