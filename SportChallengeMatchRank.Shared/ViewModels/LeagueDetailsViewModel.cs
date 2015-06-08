using Xamarin.Forms;
using System.Threading.Tasks;
using System;
using System.Linq;

[assembly: Dependency(typeof(SportChallengeMatchRank.Shared.LeagueDetailsViewModel))]

namespace SportChallengeMatchRank.Shared
{
	public class LeagueDetailsViewModel : BaseViewModel
	{
		#region Properties

		public string CreatedBy
		{
			get
			{
				return League == null || League.CreatedByAthlete == null ? null : "created on {0} by {1}".Fmt(League.DateCreated.Value.ToString("MMMM dd, yyyy"), League.CreatedByAthlete.Name);
			}
		}

		public bool IsMember
		{
			get
			{
				return App.CurrentAthlete != null && League != null && App.CurrentAthlete.Memberships.Any(m => m.League.Id == League.Id);
			}
		}

		public bool IsNotLeader
		{
			get
			{
				if(League.Memberships.Count == 0)
					return true;
				
				return LeaderMembership?.AthleteId != App.CurrentAthlete.Id;
			}
		}

		public Membership LeaderMembership
		{
			get
			{
				return League.Memberships.OrderBy(m => m.CurrentRank).FirstOrDefault();
			}
		}

		public Membership CurrentMembership
		{
			get
			{
				if(!IsMember)
					return null;

				var membership = DataManager.Instance.Memberships.Values.SingleOrDefault(m => m.LeagueId == League.Id && m.AthleteId == App.CurrentAthlete.Id);

				if(membership == null)
				{
					
				}

				return membership;
			}
		}

		public bool CanGetRules
		{
			get
			{
				return !string.IsNullOrWhiteSpace(League.RulesUrl);	
			}
		}

		MembershipDetailsViewModel _membershipViewModel;

		public MembershipDetailsViewModel MembershipViewModel
		{
			get
			{
				if(_membershipViewModel == null)
				{
					if(IsMember)
					{
						_membershipViewModel = new MembershipDetailsViewModel {
							MembershipId = App.CurrentAthlete.Memberships.First(m => m.LeagueId == League.Id).Id
						};
					}
				}

				return _membershipViewModel;
			}
		}

		public string DateRange
		{
			get
			{
				if(League == null)
					return null;
				
				var range = "open season";

				if(League != null && League.StartDate.HasValue)
					range = "beginning {0}".Fmt(League.StartDate.Value.ToString("MMM dd, yyyy"));

				if(League != null && League.EndDate.HasValue)
					range += "- {0}".Fmt(League.EndDate.Value.ToString("MMM dd, yyyy"));

				return range;
			}
		}

		public LeagueViewModel LeagueViewModel
		{
			get;
			set;
		}

		public ChallengeDetailsViewModel OngoingChallengeViewModel
		{
			get;
			set;
		}

		public ChallengeDetailsViewModel PreviousChallengeViewModel
		{
			get;
			set;
		}

		League _league;

		public League League
		{
			get
			{
				return _league;
			}
			set
			{
				SetPropertyChanged(ref _league, value);
				LeagueViewModel = new LeagueViewModel(_league);

				if(_league != null)
					App.Current.GetTheme(_league);
				
				NotifyPropertiesChanged();
			}
		}

		#endregion

		async public Task LoadAthlete()
		{
			await RunSafe(AzureService.Instance.GetAthleteById(League.CreatedByAthleteId));
			League.RefreshMemberships();

			League.SetPropertyChanged("CreatedByAthlete");
			NotifyPropertiesChanged();
		}

		async public Task<bool> JoinLeague()
		{
			var membership = new Membership {
				AthleteId = App.CurrentAthlete.Id,
				LeagueId = League.Id,
				CurrentRank = 0,
			};

			var task = AzureService.Instance.SaveMembership(membership);
			await RunSafe(task);

			NotifyPropertiesChanged();
			return task.IsCompleted && !task.IsFaulted;
		}

		async public Task LeaveLeague()
		{
			var membership = App.CurrentAthlete.Memberships.SingleOrDefault(m => m.LeagueId == League.Id);
			await RunSafe(AzureService.Instance.DeleteMembership(membership.Id));
			NotifyPropertiesChanged();
		}

		async public Task RefreshLeague()
		{
			if(IsBusy)
				return;

			using(new Busy(this))
			{
				Console.WriteLine("Refreshing League!!!");
				var task = AzureService.Instance.GetLeagueById(League.Id, true);
				await RunSafe(task);

				if(task.IsFaulted)
					return;

				League = task.Result;
				NotifyPropertiesChanged();
			}
		}

		public void NotifyPropertiesChanged()
		{
			CurrentMembership.LocalRefresh();

			if(CurrentMembership?.OngoingChallenge == null)
				OngoingChallengeViewModel = null;

			if(CurrentMembership?.OngoingChallenge != null)
				OngoingChallengeViewModel = new ChallengeDetailsViewModel(CurrentMembership?.OngoingChallenge);

			SetPropertyChanged("DateRange");
			SetPropertyChanged("CreatedBy");
			SetPropertyChanged("IsMember");
			SetPropertyChanged("MembershipViewModel");
			SetPropertyChanged("CurrentMembership");
			SetPropertyChanged("OngoingChallengeViewModel");
			SetPropertyChanged("PreviousChallengeViewModel");
			SetPropertyChanged("LeaderMembership");
			SetPropertyChanged("IsNotLeader");

			CurrentMembership?.OngoingChallenge?.NotifyPropertiesChanged();
			CurrentMembership?.OngoingChallenge?.NotifyPropertiesChanged();

			MembershipViewModel?.NotifyPropertiesChanged();
			OngoingChallengeViewModel?.NotifyPropertiesChanged();
			PreviousChallengeViewModel?.NotifyPropertiesChanged();
		}
	}
}