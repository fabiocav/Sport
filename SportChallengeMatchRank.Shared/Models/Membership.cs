using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
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
		public const string AthleteIdPropertyName = "AthleteId";

		public string AthleteId
		{
			get
			{
				return _athleteId;
			}
			set
			{
				SetProperty(ref _athleteId, value, AthleteIdPropertyName);
				OnPropertyChanged("Athlete");
			}
		}

		string _leagueId;
		public const string LeagueIdPropertyName = "LeagueId";

		public string LeagueId
		{
			get
			{
				return _leagueId;
			}
			set
			{
				SetProperty(ref _leagueId, value, LeagueIdPropertyName);
				OnPropertyChanged("League");
			}
		}

		int _currentRank;
		public const string CurrentRankPropertyName = "CurrentRank";

		public int CurrentRank
		{
			get
			{
				return _currentRank;
			}
			set
			{
				SetProperty(ref _currentRank, value, CurrentRankPropertyName);
			}
		}

		bool _isAdmin;
		public const string IsAdminPropertyName = "IsAdmin";

		public bool IsAdmin
		{
			get
			{
				return _isAdmin;
			}
			set
			{
				SetProperty(ref _isAdmin, value, IsAdminPropertyName);
			}
		}

		public void LocalRefresh()
		{
			if(Athlete != null)
				Athlete.RefreshMemberships();

			if(League != null)
				League.RefreshMemberships();
		}

		public Challenge GetExistingChallengeWithAthlete(Athlete athlete)
		{
			if(Athlete == null || athlete.Id == Athlete.Id)
				return null;

			//Check to see if they are part of the same league
			var membership = athlete.Memberships.SingleOrDefault(m => m.LeagueId == LeagueId);

			if(membership != null)
			{
				return athlete.Challenges.FirstOrDefault(c => (c.ChallengeeAthleteId == athlete.Id ||
					c.ChallengerAthleteId == athlete.Id) && c.LeagueId == LeagueId);
			}

			return null;
		}

		public bool HasExistingChallengeWithAthlete(Athlete athlete)
		{
			return GetExistingChallengeWithAthlete(athlete) != null;
		}

		public bool CanChallengeAthlete(Athlete athlete)
		{
			if(Athlete == null || athlete.Id == Athlete.Id)
				return false;
			
			bool canChallenge = false;

			//Check to see if they are part of the same league
			var membership = athlete.Memberships.SingleOrDefault(m => m.LeagueId == LeagueId);

			if(membership != null)
			{
				//Ensure they are within range and lower in rank than the challengee
				var diff = membership.CurrentRank - CurrentRank;
				canChallenge = diff > 0 && diff <= League.MaxChallengeRange;
			}

			if(canChallenge)
			{
				//Athlete is within range but let's make sure there aren't already challenges out there
				var alreadyChallenged = athlete.Challenges.Any(c => (c.ChallengeeAthleteId == athlete.Id ||
					                        c.ChallengerAthleteId == athlete.Id) && c.LeagueId == LeagueId);

				canChallenge = !alreadyChallenged;
			}

			Console.WriteLine("CanChallenge " + canChallenge);
			return canChallenge;
		}
	}
}