using System;
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

		public bool HasStarted
		{
			get
			{
				return hasStarted;
			}
			set
			{
				ProcPropertyChanged(ref hasStarted, value);
			}
		}

		string _name;

		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				ProcPropertyChanged(ref _name, value);
			}
		}

		string _sport;

		public string Sport
		{
			get
			{
				return _sport;
			}
			set
			{
				ProcPropertyChanged(ref _sport, value);
			}
		}

		bool _isEnabled;

		public bool IsEnabled
		{
			get
			{
				return _isEnabled;
			}
			set
			{
				ProcPropertyChanged(ref _isEnabled, value);
			}
		}

		bool _isAcceptingMembers;

		public bool IsAcceptingMembers
		{
			get
			{
				return _isAcceptingMembers;
			}
			set
			{
				ProcPropertyChanged(ref _isAcceptingMembers, value);
			}
		}

		int _maxChallengeRange;

		public int MaxChallengeRange
		{
			get
			{
				return _maxChallengeRange;
			}
			set
			{
				ProcPropertyChanged(ref _maxChallengeRange, value);
			}
		}

		int _minHoursBetweenChallenge;

		public int MinHoursBetweenChallenge
		{
			get
			{
				return _minHoursBetweenChallenge;
			}
			set
			{
				ProcPropertyChanged(ref _minHoursBetweenChallenge, value);
			}
		}

		int _matchGameCount;

		public int MatchGameCount
		{
			get
			{
				return _matchGameCount;
			}
			set
			{
				ProcPropertyChanged(ref _matchGameCount, value);
			}
		}

		string description;

		public string Description
		{
			get
			{
				return description;
			}
			set
			{
				ProcPropertyChanged(ref description, value);
			}
		}

		int _season;

		public int Season
		{
			get
			{
				return _season;
			}
			set
			{
				ProcPropertyChanged(ref _season, value);
			}
		}

		ObservableCollection<Membership> _memberships = new ObservableCollection<Membership>();

		public ObservableCollection<Membership> Memberships
		{
			get
			{
				return _memberships;
			}
			set
			{
				ProcPropertyChanged(ref _memberships, value);
			}
		}

		string imageUrl;

		public string ImageUrl
		{
			get
			{
				return imageUrl;
			}
			set
			{
				ProcPropertyChanged(ref imageUrl, value);
			}
		}

		string createdByAthleteId;

		public string CreatedByAthleteId
		{
			get
			{
				return createdByAthleteId;
			}
			set
			{
				ProcPropertyChanged(ref createdByAthleteId, value);
				SetPropertyChanged("CreatedByAthlete");
			}
		}

		DateTime? _startDate;

		public DateTime? StartDate
		{
			get
			{
				return _startDate;
			}
			set
			{
				ProcPropertyChanged(ref _startDate, value);
				SetPropertyChanged("DateRange");
			}
		}

		DateTime? _endDate;

		public DateTime? EndDate
		{
			get
			{
				return _endDate;
			}
			set
			{
				ProcPropertyChanged(ref _endDate, value);
				SetPropertyChanged("DateRange");
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