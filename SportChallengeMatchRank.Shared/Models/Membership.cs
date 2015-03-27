using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace SportChallengeMatchRank.Shared
{
	public class Membership : BaseModel
	{
		ObservableCollection<Athlete> _athletes;
		public const string AthletesPropertyName = "Athletes";

		public ObservableCollection<Athlete> Athletes
		{
			get
			{
				return _athletes;
			}
			set
			{
				SetProperty(ref _athletes, value, AthletesPropertyName);
			}
		}

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
	}
}