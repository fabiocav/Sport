using System;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;

namespace SportChallengeMatchRank.Shared
{
	public class Membership : BaseModel
	{
		[JsonIgnore]
		public League League
		{
			get
			{
				return LeagueId == null ? null : DataManager.Instance.Leagues.Get(LeagueId);
			}
		}

		[JsonIgnore]
		public Athlete Athlete
		{
			get
			{
				return AthleteId == null ? null : DataManager.Instance.Athletes.Get(AthleteId);
			}
		}

		DateTimeOffset? _abandonDate;

		public DateTimeOffset? AbandonDate
		{
			get
			{
				return _abandonDate;
			}
			set
			{
				SetPropertyChanged(ref _abandonDate, value);
				SetPropertyChanged("IsAbandoned");
			}
		}

		public bool IsAbandoned
		{
			get
			{
				return AbandonDate.HasValue;
			}
		}

		string _athleteId;

		public string AthleteId
		{
			get
			{
				return _athleteId;
			}
			set
			{
				SetPropertyChanged(ref _athleteId, value);
				SetPropertyChanged("Athlete");
			}
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

		int _currentRank;

		public int CurrentRank
		{
			get
			{
				return _currentRank;
			}
			set
			{
				SetPropertyChanged(ref _currentRank, value);
				SetPropertyChanged("CurrentRankDisplay");
				SetPropertyChanged("CurrentRankOrdinal");
			}
		}

		public int CurrentRankDisplay
		{
			get
			{
				return CurrentRank + 1;
			}
		}

		public string CurrentRankOrdinal
		{
			get
			{
				return CurrentRankDisplay.ToOrdinal();
			}
		}

		DateTime? _lastRankChange;

		public DateTime? LastRankChange
		{
			get
			{
				return _lastRankChange;
			}
			set
			{
				SetPropertyChanged(ref _lastRankChange, value);
			}
		}

		public Challenge OngoingChallenge
		{
			get
			{
				return League?.OngoingChallenges?.FirstOrDefault(c => c.InvolvesAthlete(AthleteId));
			}
		}

		bool _isAdmin;

		public bool IsAdmin
		{
			get
			{
				return _isAdmin;
			}
			set
			{
				SetPropertyChanged(ref _isAdmin, value);
			}
		}

		[JsonIgnore]
		public DateTime LastRankChangeDate
		{
			get
			{
				return LastRankChange == null ? DateCreated.Value : LastRankChange.Value; 
			}
		}

		public string RankDescription
		{
			get
			{
				var dayCount = Math.Round(DateTime.UtcNow.Subtract(LastRankChangeDate).TotalDays);
				return "{0} out of {1} for {2} day{3}".Fmt(CurrentRankDisplay.ToOrdinal(), League.Memberships.Count, dayCount, dayCount == 1 ? "" : "s");
			}
		}

		public void LocalRefresh()
		{
			if(Athlete != null)
				Athlete.RefreshMemberships();

			if(League != null)
			{
				League.RefreshMemberships();
				League.RefreshChallenges();
			}

			SetPropertyChanged("LastRankChangeDate");
			SetPropertyChanged("CurrentRank");
			SetPropertyChanged("OngoingChallenge");
		}

		public Challenge GetOngoingChallenge(Athlete athlete)
		{
			if(Athlete == null || athlete.Id == Athlete.Id)
				return null;

			//Check to see if they are part of the same league
			var membership = athlete.Memberships.SingleOrDefault(m => m.LeagueId == LeagueId);
			return membership != null ? League.OngoingChallenges.InvolvingAthlete(athlete.Id) : null;
		}

		public bool HasExistingChallengeWithAthlete(Athlete athlete)
		{
			return GetOngoingChallenge(athlete) != null;
		}

		public string GetChallengeConflictReason(Athlete athlete)
		{
			if(Athlete == null || athlete.Id == Athlete.Id)
				return "You cannot challenge yourself";

			//Check to see if they are part of the same league
			var membership = athlete.Memberships.SingleOrDefault(m => m.LeagueId == LeagueId);

			if(membership != null)
			{
				//Ensure they are within range and lower in rank than the challengee
				var diff = membership.CurrentRank - CurrentRank;
				if(diff <= 0 || diff > League.MaxChallengeRange)
				{
					return "{0} is not within a valid range of being challenged".Fmt(Athlete.Alias);
				}
			}
			else
			{
				return "{0} is not a member of the {1} league".Fmt(Athlete.Alias, League.Name);
			}


			var challenge = GetOngoingChallenge(App.CurrentAthlete);
			if(challenge != null)
			{
				var other = challenge.ChallengeeAthleteId == athlete.Id ? challenge.ChallengerAthlete : challenge.ChallengeeAthlete;
				return "You already have an ongoing challenge with {0}".Fmt(other.Alias);
			}

			//Athlete is within range but let's make sure there aren't already challenges out there 
			challenge = League.OngoingChallenges.FirstOrDefault(c => c.InvolvesAthlete(athlete.Id));
			if(challenge != null)
			{
				var other = challenge.ChallengeeAthleteId == Athlete.Id ? challenge.ChallengerAthlete : challenge.ChallengeeAthlete;
				return "{0} already has an ongoing challenge with {1}".Fmt(Athlete.Alias, other.Alias);
			}

			return null;
		}

		public bool CanChallengeAthlete(Athlete athlete)
		{
			return GetChallengeConflictReason(athlete) == null;
		}
	}

	public class MembershipComparer : IEqualityComparer<Membership>
	{
		public bool Equals(Membership x, Membership y)
		{
			var isEqual = x?.Id == y?.Id && x?.UpdatedAt == y?.UpdatedAt && x?.CurrentRank == y?.CurrentRank;

			if(isEqual && x.OngoingChallenge != null && y.OngoingChallenge != null)
				isEqual = x.OngoingChallenge.Id == y.OngoingChallenge.Id;

			if((x.OngoingChallenge == null && y.OngoingChallenge != null) || (x.OngoingChallenge != null && y.OngoingChallenge == null))
				return false;

			return isEqual;
		}

		public int GetHashCode(Membership obj)
		{
			return obj.Id != null ? obj.Id.GetHashCode() : base.GetHashCode();
		}
	}
}