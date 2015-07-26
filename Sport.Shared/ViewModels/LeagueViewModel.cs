using System.Threading.Tasks;
using System.Linq;

namespace Sport.Shared
{
	public class LeagueViewModel : BaseViewModel
	{
		public LeagueViewModel(League league, Athlete athlete = null, bool isLast = false)
		{
			League = league;
			Athlete = athlete;
			IsLast = isLast;
			LocalRefresh();
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
			}
		}

		public Athlete Athlete
		{
			get;
			set;
		}

		public bool HasChallenge
		{
			get
			{
				return Membership != null && Membership.OngoingChallenge != null;
			}
		}

		public Membership Membership
		{
			get
			{
				return Athlete?.Memberships.FirstOrDefault(m => m.LeagueId == League.Id);
			}
		}

		public bool IsFirstPlace
		{
			get
			{
				return App.CurrentAthlete.Memberships.Any(m => m.LeagueId == League.Id && m.CurrentRank == 0 && m.League != null && m.League.HasStarted);
			}
		}

		public bool IsMember
		{
			get
			{
				return App.CurrentAthlete.Memberships.Any(m => m.LeagueId == League.Id);
			}
		}

		public bool IsNotMemberAndLeagueStarted
		{
			get
			{
				return !IsMember && League.HasStarted;
			}
		}

		public bool IsMemberAndLeagueStarted
		{
			get
			{
				return IsMember && League.HasStarted;
			}
		}

		bool _isLast;

		public bool IsLast
		{
			get
			{
				return _isLast;
			}
			set
			{
				SetPropertyChanged(ref _isLast, value);
			}
		}

		public void LocalRefresh()
		{
			SetPropertyChanged("HasChallenge");
			SetPropertyChanged("Membership");
			SetPropertyChanged("IsMember");
			SetPropertyChanged("League");
			SetPropertyChanged("IsLast");
			SetPropertyChanged("IsMemberAndLeagueStarted");
			SetPropertyChanged("IsNotMemberAndLeagueStarted");
			SetPropertyChanged("IsFirstPlace");
		}

		async public Task GetAllMemberships(bool forceRefresh = false)
		{
			if(!forceRefresh)
				return;

			using(new Busy(this))
			{
				var task = AzureService.Instance.GetAllAthletesForLeague(League);
				await RunSafe(task);
				League.RefreshMemberships();
				LocalRefresh();
			}

			IsBusy = false;
		}
	}
}