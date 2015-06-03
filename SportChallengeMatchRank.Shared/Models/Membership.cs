using System;
using Newtonsoft.Json;
using System.Linq;

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
			}
		}

		public int CurrentRankDisplay
		{
			get
			{
				return CurrentRank + 1;
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
				League.RefreshMemberships();

			SetPropertyChanged("OngoingChallenge");
			SetPropertyChanged("LastRankChangeDate");
			SetPropertyChanged("CurrentRank");
		}

		public Challenge GetExistingOngoingChallengeWithAthlete(Athlete athlete)
		{
			if(Athlete == null || athlete.Id == Athlete.Id)
				return null;

			//Check to see if they are part of the same league
			var membership = athlete.Memberships.SingleOrDefault(m => m.LeagueId == LeagueId);

			if(membership != null)
			{
				return athlete.AllChallenges.FirstOrDefault(c => (c.ChallengeeAthleteId == athlete.Id ||
				c.ChallengerAthleteId == athlete.Id) && c.LeagueId == LeagueId && !c.IsCompleted);
			}

			return null;
		}

		public bool HasExistingChallengeWithAthlete(Athlete athlete)
		{
			return GetExistingOngoingChallengeWithAthlete(athlete) != null;
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
					return "{0} is not within a valid range of being challenged.".Fmt(Athlete.Alias);
				}
			}
			else
			{
				return "{0} is not a member of the {1} league".Fmt(Athlete.Alias, League.Name);
			}

			var challenge = athlete.AllChallenges.FirstOrDefault(c => c.LeagueId == LeagueId && !c.IsCompleted);
			if(challenge != null)
			{
				var other = challenge.ChallengeeAthleteId == athlete.Id ? challenge.ChallengerAthlete : challenge.ChallengeeAthlete;
				return "You already have an ongoing challenge with {0}.".Fmt(other.Alias);
			}

			//Athlete is within range but let's make sure there aren't already challenges out there 
			challenge = Athlete.AllChallenges.FirstOrDefault(c => c.LeagueId == LeagueId && !c.IsCompleted);
			if(challenge != null)
			{
				var other = challenge.ChallengeeAthleteId == Athlete.Id ? challenge.ChallengerAthlete : challenge.ChallengeeAthlete;
				return "{0} already has an ongoing challenge with {1}.".Fmt(Athlete.Alias, other.Alias);
			}

			return null;
		}

		public bool CanChallengeAthlete(Athlete athlete)
		{
			return GetChallengeConflictReason(athlete) == null;
		}
	}
}