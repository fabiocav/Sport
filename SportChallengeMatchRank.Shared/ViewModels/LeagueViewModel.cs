﻿using System.Threading.Tasks;
using System.Linq;

namespace SportChallengeMatchRank.Shared
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

		public League League
		{
			get;
			set;
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

		public bool IsMember
		{
			get
			{
				return App.CurrentAthlete.Memberships.Any(m => m.LeagueId == League.Id);
			}
		}

		public string MemberCount
		{
			get
			{
				return "There {2} {0} member{1} enjoying sport".Fmt(League.Memberships.Count, League.Memberships.Count == 1 ? "" : "s", League.Memberships.Count == 1 ? "is" : "are");
			}
		}

		public bool IsMemberAndStarted
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
			SetPropertyChanged("MemberCount");
			SetPropertyChanged("IsMemberAndStarted");
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