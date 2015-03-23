using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SportRankerMatchOn.Shared
{
	public class Membership : BaseModel
	{
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
			}
		}

		Athlete _athlete;
		public const string AthletePropertyName = "Athlete";

		[JsonIgnore]
		public Athlete Athlete
		{
			get
			{
				return _athlete;
			}
			set
			{
				SetProperty(ref _athlete, value, AthletePropertyName);
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
			}
		}

		League _league;
		public const string LeaguePropertyName = "League";

		[JsonIgnore]
		public League League
		{
			get
			{
				return _league;
			}
			set
			{
				SetProperty(ref _league, value, LeaguePropertyName);
			}
		}

		DateTime _joinDate;
		public const string JoinDatePropertyName = "JoinDate";

		public DateTime JoinDate
		{
			get
			{
				return _joinDate;
			}
			set
			{
				SetProperty(ref _joinDate, value, JoinDatePropertyName);
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
	}
}