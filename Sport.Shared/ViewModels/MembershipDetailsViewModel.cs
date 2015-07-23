using System;
using System.Windows.Input;
using Xamarin.Forms;
using System.Threading.Tasks;

[assembly: Dependency(typeof(Sport.Shared.MembershipDetailsViewModel))]

namespace Sport.Shared
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
				SetPropertyChanged("IsCurrentAthlete");
			}
		}

		public bool IsCurrentAthlete
		{
			get
			{
				return Membership.AthleteId == App.CurrentAthlete.Id;
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
				var challenge = Membership.GetOngoingChallenge(App.CurrentAthlete);
				return challenge != null && challenge.ChallengerAthleteId == App.CurrentAthlete.Id && challenge.ChallengeeAthleteId == Membership.AthleteId
				&& Membership.LeagueId == challenge.LeagueId;
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

		public string Stats
		{
			get
			{
				return "W 14 L 7 ~ .5 (mock)";
			}
		}

		public void NotifyPropertiesChanged()
		{
			SetPropertyChanged("CanChallenge");
			SetPropertyChanged("CanRevokeChallenge");
			SetPropertyChanged("Membership");
			SetPropertyChanged("CanDeleteMembership");
			SetPropertyChanged("RankDescription");
			SetPropertyChanged("Stats");
			SetPropertyChanged("JoinDescription");
			SetPropertyChanged("IsCurrentAthlete");
			Membership?.LocalRefresh();
		}

		public void LocalRefresh()
		{
			Membership.Athlete.RefreshMemberships();
		}

		//		public ICommand SaveCommand
		//		{
		//			get
		//			{
		//				return new Command(async(param) =>
		//					await SaveMembership());
		//			}
		//		}
		//
		//		async public Task SaveMembership()
		//		{
		//			await RunSafe(AzureService.Instance.SaveMembership(Membership));
		//		}

		async public Task DeleteMembership()
		{
			await RunSafe(AzureService.Instance.DeleteMembership(Membership.Id));
		}

		async public Task RefreshMembership()
		{
			using(new Busy(this))
			{
				var task = AzureService.Instance.GetMembershipById(MembershipId, true);
				await RunSafe(task);

				if(task.IsFaulted)
					return;
			}
			NotifyPropertiesChanged();
		}
	}
}