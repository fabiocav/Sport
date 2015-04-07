﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Linq;

namespace SportChallengeMatchRank.Shared
{
	public class League : BaseModel
	{
		public League()
		{
			Initialize();
		}

		#region Properties

		[JsonIgnore]
		public Athlete CreatedByAthlete
		{
			get
			{
				return CreatedByAthleteId == null ? null : DataManager.Instance.Athletes.Get(CreatedByAthleteId);
			}
		}

		public List<string> MembershipIds
		{
			get;
			set;
		}

		bool hasStarted;
		public const string HasStartedPropertyName = "HasStarted";

		public bool HasStarted
		{
			get
			{
				return hasStarted;
			}
			set
			{
				SetProperty(ref hasStarted, value, HasStartedPropertyName);
			}
		}

		string _name;
		public const string NamePropertyName = "Name";

		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				SetProperty(ref _name, value, NamePropertyName);
			}
		}

		string _sport;
		public const string SportPropertyName = "Sport";

		public string Sport
		{
			get
			{
				return _sport;
			}
			set
			{
				SetProperty(ref _sport, value, SportPropertyName);
			}
		}

		bool _isEnabled;
		public const string IsEnabledPropertyName = "IsEnabled";

		public bool IsEnabled
		{
			get
			{
				return _isEnabled;
			}
			set
			{
				SetProperty(ref _isEnabled, value, IsEnabledPropertyName);
			}
		}

		bool _isAcceptingMembers;
		public const string IsAcceptingMembersPropertyName = "IsAcceptingMembers";

		public bool IsAcceptingMembers
		{
			get
			{
				return _isAcceptingMembers;
			}
			set
			{
				SetProperty(ref _isAcceptingMembers, value, IsAcceptingMembersPropertyName);
			}
		}

		int _maxChallengeRange;
		public const string MaxChallengeRangePropertyName = "MaxChallengeRange";

		public int MaxChallengeRange
		{
			get
			{
				return _maxChallengeRange;
			}
			set
			{
				SetProperty(ref _maxChallengeRange, value, MaxChallengeRangePropertyName);
			}
		}

		int _minHoursBetweenChallenge;
		public const string MinHoursBetweenChallengePropertyName = "MinHoursBetweenChallenge";

		public int MinHoursBetweenChallenge
		{
			get
			{
				return _minHoursBetweenChallenge;
			}
			set
			{
				SetProperty(ref _minHoursBetweenChallenge, value, MinHoursBetweenChallengePropertyName);
			}
		}

		int _matchGameCount;
		public const string MatchGameCountPropertyName = "MatchGameCount";

		public int MatchGameCount
		{
			get
			{
				return _matchGameCount;
			}
			set
			{
				SetProperty(ref _matchGameCount, value, MatchGameCountPropertyName);
			}
		}

		string description;
		public const string DescriptionPropertyName = "Description";

		public string Description
		{
			get
			{
				return description;
			}
			set
			{
				SetProperty(ref description, value, DescriptionPropertyName);
			}
		}

		int _season;
		public const string SeasonPropertyName = "Season";

		public int Season
		{
			get
			{
				return _season;
			}
			set
			{
				SetProperty(ref _season, value, SeasonPropertyName);
			}
		}

		ObservableCollection<Membership> _memberships = new ObservableCollection<Membership>();
		public const string MembershipsPropertyName = "Memberships";

		public ObservableCollection<Membership> Memberships
		{
			get
			{
				return _memberships;
			}
			set
			{
				SetProperty(ref _memberships, value, MembershipsPropertyName);
			}
		}

		string imageUrl;
		public const string ImageUrlPropertyName = "ImageUrl";

		public string ImageUrl
		{
			get
			{
				return imageUrl;
			}
			set
			{
				SetProperty(ref imageUrl, value, ImageUrlPropertyName);
			}
		}

		string createdByAthleteId;
		public const string CreatedByAthleteIdPropertyName = "CreatedByAthleteId";

		public string CreatedByAthleteId
		{
			get
			{
				return createdByAthleteId;
			}
			set
			{
				SetProperty(ref createdByAthleteId, value, CreatedByAthleteIdPropertyName);
				OnPropertyChanged("CreatedByAthlete");
			}
		}

		DateTime? _startDate;
		public const string StartDatePropertyName = "StartDate";

		public DateTime? StartDate
		{
			get
			{
				return _startDate;
			}
			set
			{
				SetProperty(ref _startDate, value, StartDatePropertyName);
				OnPropertyChanged("DateRange");
			}
		}

		DateTime? _endDate;
		public const string EndDatePropertyName = "EndDate";

		public DateTime? EndDate
		{
			get
			{
				return _endDate;
			}
			set
			{
				SetProperty(ref _endDate, value, EndDatePropertyName);
				OnPropertyChanged("DateRange");
			}
		}

		#endregion

		void Initialize()
		{
			StartDate = DateTime.Now.AddDays(7);
			EndDate = DateTime.Now.AddMonths(6);
			Memberships = new ObservableCollection<Membership>();

			MaxChallengeRange = 2;
			MinHoursBetweenChallenge = 48;
			MatchGameCount = 3;

			IsAcceptingMembers = true;
			IsEnabled = true;
		}

		public void RefreshMemberships()
		{
			_memberships.Clear();
			DataManager.Instance.Memberships.Values.Where(m => m.LeagueId == Id).OrderBy(m => m.CurrentRank).ToList().ForEach(_memberships.Add);
		}
	}

	public enum Sports
	{
		TableTennis,
		Foosball,
	}
}