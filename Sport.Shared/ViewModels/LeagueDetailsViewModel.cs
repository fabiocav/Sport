using Xamarin.Forms;
using System.Threading.Tasks;
using System;
using System.Linq;

[assembly: Dependency(typeof(Sport.Shared.LeagueDetailsViewModel))]

namespace Sport.Shared
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

		public bool HasLeaderOtherThanSelf
		{
			get
			{
				if(League.Memberships.Count == 0)
					return false;
				
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

		Membership _currentMembership;

		public Membership CurrentMembership
		{
			get
			{
				if(!IsMember)
					return null;

				if(_currentMembership == null)
					_currentMembership = DataManager.Instance.Memberships.Values.SingleOrDefault(m => m.LeagueId == League.Id && m.AthleteId == App.CurrentAthlete.Id);

				return _currentMembership;
			}
		}

		public bool IsFirstPlace
		{
			get
			{
				return CurrentMembership != null && CurrentMembership.CurrentRank == 0 && CurrentMembership.League.HasStarted;
			}
		}

		public bool CanGetRules
		{
			get
			{
				return !string.IsNullOrWhiteSpace(League.RulesUrl);	
			}
		}

		string _praisePhrase;

		public string PraisePhrase
		{
			get
			{
				if(_praisePhrase == null)
				{
					var random = new Random().Next(0, App.PraisePhrases.Count - 1);
					_praisePhrase = App.PraisePhrases[random];
				}
				return "you're {0}".Fmt(_praisePhrase);
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

		public bool CanChallenge
		{
			get
			{
				return GetBestChallengee != null;
			}
		}

		public Membership GetBestChallengee
		{
			get
			{
				if(!League.HasStarted && CurrentMembership != null)
					return null;
				
				var gap = League.MaxChallengeRange;
				Membership best = null;
				while(best == null && gap > 0 && CurrentMembership != null)
				{
					best = League.Memberships.SingleOrDefault(m => m.CurrentRank == CurrentMembership.CurrentRank - gap);

					if(best == null)
						return null;
					
					//Ensure no issues with player
					var conflict = best.GetChallengeConflictReason(CurrentMembership.Athlete);
					if(best != null && conflict != null)
						best = null;

					gap--;
				}

				return best;
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
				var diff = _league != value;
				SetPropertyChanged(ref _league, value);

				if(diff)
				{
					_praisePhrase = null;
					_currentMembership = null;
					SetPropertyChanged("CurrentMembership");
					LeagueViewModel = new LeagueViewModel(_league.Id);
				}
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

			if(task.IsCompleted && !task.IsFaulted)
			{
				var regTask = AzureService.Instance.UpdateAthleteNotificationHubRegistration(App.CurrentAthlete, true);
				await RunSafe(regTask);
			}

			NotifyPropertiesChanged();
			return task.IsCompleted && !task.IsFaulted;
		}

		async public Task LeaveLeague()
		{
			var membership = App.CurrentAthlete.Memberships.SingleOrDefault(m => m.LeagueId == League.Id);

			var task = AzureService.Instance.DeleteMembership(membership.Id);
			await RunSafe(task);

			if(task.IsCompleted && !task.IsFaulted)
			{
				var regTask = AzureService.Instance.UpdateAthleteNotificationHubRegistration(App.CurrentAthlete, true);
				await RunSafe(regTask);
			}

			NotifyPropertiesChanged();
		}

		async public Task RefreshLeague(bool force = false)
		{
			if(IsBusy)
				return;

			using(new Busy(this))
			{
				var task = AzureService.Instance.GetLeagueById(League.Id, true);
				await RunSafe(task);

				if(task.IsFaulted)
					return;

				if(force || (League == null || !League.Equals(task.Result)))
				{
					task.Result.Theme = League?.Theme;
					_praisePhrase = null;
					League = task.Result;
					NotifyPropertiesChanged();
				}
			}
		}

		public void NotifyPropertiesChanged()
		{
			CurrentMembership?.LocalRefresh();

			if(CurrentMembership?.OngoingChallenge == null)
				OngoingChallengeViewModel = null;

			if(CurrentMembership?.OngoingChallenge != null && OngoingChallengeViewModel == null)
			{
				OngoingChallengeViewModel = new ChallengeDetailsViewModel {
					Challenge = CurrentMembership?.OngoingChallenge
				};
			}
			else if(OngoingChallengeViewModel != null)
			{
				OngoingChallengeViewModel.Challenge = CurrentMembership?.OngoingChallenge;
			}

			SetPropertyChanged("DateRange");
			SetPropertyChanged("CreatedBy");
			SetPropertyChanged("IsMember");
			SetPropertyChanged("LeaderMembership");
			SetPropertyChanged("HasLeaderOtherThanSelf");
			SetPropertyChanged("MembershipViewModel");
			SetPropertyChanged("CurrentMembership");
			SetPropertyChanged("CanChallenge");
			SetPropertyChanged("OngoingChallengeViewModel");
			SetPropertyChanged("PreviousChallengeViewModel");
			SetPropertyChanged("GetBestChallengee");
			SetPropertyChanged("IsFirstPlace");
			SetPropertyChanged("PraisePhrase");

			CurrentMembership?.OngoingChallenge?.NotifyPropertiesChanged();
			MembershipViewModel?.NotifyPropertiesChanged();
			OngoingChallengeViewModel?.NotifyPropertiesChanged();
			PreviousChallengeViewModel?.NotifyPropertiesChanged();
		}
	}
}