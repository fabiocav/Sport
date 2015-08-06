using System.Threading.Tasks;
using System.Linq;

namespace Sport.Shared
{
	public class LeagueViewModel : BaseViewModel
	{
		public LeagueViewModel(League league, Athlete athlete = null, bool isLast = false)
		{
			LeagueId = league.Id;
			Athlete = athlete;
			IsLast = isLast;
			LocalRefresh();
		}

		string _leagueId;

		public string LeagueId
		{
			get
			{
				return _leagueId;
			}
			set
			{
				SetPropertyChanged(ref _leagueId, value);
			}
		}

		public League League
		{
			get
			{
				return LeagueId == null ? null : DataManager.Instance.Leagues.Get(LeagueId);
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
				if(League == null || App.CurrentAthlete.Memberships == null)
					return false;
				
				return App.CurrentAthlete.Memberships.Any(m => m.LeagueId == League.Id && m.CurrentRank == 0 && m.League != null && m.League.HasStarted);
			}
		}

		public bool IsMember
		{
			get
			{
				if(League == null || App.CurrentAthlete.Memberships == null)
					return false;

				return App.CurrentAthlete.Memberships.Any(m => m.LeagueId == League.Id);
			}
		}

		public bool IsNotMemberAndLeagueStarted
		{
			get
			{
				if(League == null)
					return false;

				return !IsMember && League.HasStarted;
			}
		}

		public bool IsMemberAndLeagueStarted
		{
			get
			{
				if(League == null)
					return false;

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

		string _emptyMessage;

		public string EmptyMessage
		{
			get
			{
				return _emptyMessage;
			}
			set
			{
				SetPropertyChanged(ref _emptyMessage, value);
			}
		}

		public void LocalRefresh()
		{
			SetPropertyChanged("HasChallenge");
			SetPropertyChanged("Membership");
			SetPropertyChanged("IsMember");
			SetPropertyChanged("LeagueId");
			SetPropertyChanged("League");
			SetPropertyChanged("IsLast");
			SetPropertyChanged("IsMemberAndLeagueStarted");
			SetPropertyChanged("IsNotMemberAndLeagueStarted");
			SetPropertyChanged("IsFirstPlace");
			SetPropertyChanged("EmptyMessage");
		}
	}
}