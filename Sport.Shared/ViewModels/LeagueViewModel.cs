using System.Linq;

namespace Sport.Shared
{
	public class LeagueViewModel : BaseViewModel
	{
		public LeagueViewModel(string leagueId, bool isLast = false)
		{
			LeagueId = leagueId;
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
				SetPropertyChanged("League");
			}
		}

		public League League
		{
			get
			{
				return LeagueId == null ? null : DataManager.Instance.Leagues.Get(LeagueId);
			}
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
				return App.CurrentAthlete?.Memberships.FirstOrDefault(m => m.LeagueId == League.Id);
			}
		}

		public bool IsFirstPlace
		{
			get
			{
				if(Membership == null)
					return false;
				
				return Membership.League.HasStarted && Membership.CurrentRank == 0;
			}
		}

		public bool IsMember
		{
			get
			{
				return Membership != null;
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
			SetPropertyChanged("League");
			SetPropertyChanged("Athlete");
			SetPropertyChanged("IsLast");
			SetPropertyChanged("IsMemberAndLeagueStarted");
			SetPropertyChanged("IsNotMemberAndLeagueStarted");
			SetPropertyChanged("IsFirstPlace");
			SetPropertyChanged("EmptyMessage");
		}
	}
}