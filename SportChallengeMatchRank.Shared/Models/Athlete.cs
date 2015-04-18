using System;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace SportChallengeMatchRank.Shared
{
	public class Athlete : BaseModel
	{
		public Athlete()
		{
			Initialize();
		}

		public Athlete(UserProfile profile)
		{
			Name = profile.Name;
			Email = profile.Email;
			AuthenticationId = profile.UserId;
			Initialize();
		}

		void Initialize()
		{
			RefreshChallenges();
		}

		public List<string> MembershipIds
		{
			get;
			set;
		}

		public List<string> IncomingChallengeIds
		{
			get;
			set;
		}

		public List<string> OutgoingChallengeIds
		{
			get;
			set;
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

		string _email;

		public string Email
		{
			get
			{
				return _email;
			}
			set
			{
				ProcPropertyChanged(ref _email, value);
			}
		}

		string _authenticationId;

		public string AuthenticationId
		{
			get
			{
				return _authenticationId;
			}
			set
			{
				ProcPropertyChanged(ref _authenticationId, value);
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
				ProcPropertyChanged(ref _isAdmin, value);
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
				SetPropertyChanged("Leagues");
			}
		}

		string deviceToken;

		public string DeviceToken
		{
			get
			{
				return deviceToken;
			}
			set
			{
				ProcPropertyChanged(ref deviceToken, value);
			}
		}

		string devicePlatform;

		public string DevicePlatform
		{
			get
			{
				return devicePlatform;
			}
			set
			{
				ProcPropertyChanged(ref devicePlatform, value);
			}
		}

		public void RefreshMemberships()
		{
			_memberships.Clear();
			DataManager.Instance.Memberships.Values.Where(m => m.AthleteId == Id).OrderBy(l => l.League.Name).ToList().ForEach(_memberships.Add);
		}

		[JsonIgnore]
		public List<League> Leagues
		{
			get
			{
				return Memberships.Select(m => m.League).ToList();
			}
		}


		[JsonIgnore]
		public List<Challenge> IncomingChallenges
		{
			get;
			private set;
		}

		[JsonIgnore]
		public List<Challenge> OutgoingChallenges
		{
			get;
			private set;
		}

		[JsonIgnore]
		public List<Challenge> AllChallenges
		{
			get
			{
				var list = new List<Challenge>(IncomingChallenges);
				list.AddRange(OutgoingChallenges);
				return list;
			}
		}

		public void RefreshChallenges()
		{
			if(IncomingChallenges == null)
				IncomingChallenges = new List<Challenge>();

			if(OutgoingChallenges == null)
				OutgoingChallenges = new List<Challenge>();

			IncomingChallenges.Clear();
			DataManager.Instance.Challenges.Values.Where(m => m.ChallengeeAthleteId == Id).ToList().ForEach(IncomingChallenges.Add);

			OutgoingChallenges.Clear();
			DataManager.Instance.Challenges.Values.Where(m => m.ChallengerAthleteId == Id).ToList().ForEach(OutgoingChallenges.Add);
		}
	}
}