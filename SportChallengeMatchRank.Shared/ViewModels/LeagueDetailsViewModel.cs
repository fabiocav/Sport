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

		public string SportDescription
		{
			get
			{
				return League == null ? null : "the honorable pastime of {0}".Fmt(League.Sport);
			}
		}

		public bool IsMember
		{
			get
			{
				return App.CurrentAthlete != null && League != null && App.CurrentAthlete.Memberships.Any(m => m.League.Id == League.Id);
			}
		}

		public Challenge OngoingChallenge
		{
			get
			{
				var challenge = App.CurrentAthlete?.GetChallengeForLeague(League);
				return challenge;
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

		LeagueViewModel LeagueViewModel
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
			using(new Busy(this))
			{
				var task = AzureService.Instance.GetLeagueById(League.Id, true);
				await RunSafe(task);

				if(task.IsFaulted)
					return;

				League = task.Result;
				await LeagueViewModel.GetAllMemberships(true);
				NotifyPropertiesChanged();
			}
		}

		void NotifyPropertiesChanged()
		{
			SetPropertyChanged("SportDescription");
			SetPropertyChanged("DateRange");
			SetPropertyChanged("CreatedBy");
			SetPropertyChanged("IsMember");
			SetPropertyChanged("OngoingChallenge");
			SetPropertyChanged("MembershipViewModel");

			OngoingChallenge?.NotifyPropertiesChanged();
		}
	}
}