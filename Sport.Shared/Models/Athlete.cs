﻿using System;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace Sport.Shared
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
			AuthenticationId = profile.Id;
			ProfileImageUrl = profile.Picture;
			Initialize();
		}

		void Initialize()
		{
		}

		public List<string> MembershipIds
		{
			get;
			set;
		}

		string _alias;

		public string Alias
		{
			get
			{
				return _alias;
			}
			set
			{
				_alias = value;
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
				SetPropertyChanged(ref _name, value);
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
				SetPropertyChanged(ref _email, value);
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
				SetPropertyChanged(ref _authenticationId, value);
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

		List<Membership> _memberships = new List<Membership>();

		[JsonIgnore]
		public List<Membership> Memberships
		{
			get
			{
				return _memberships;
			}
			set
			{
				SetPropertyChanged(ref _memberships, value);
				SetPropertyChanged("Leagues");
			}
		}

		string _deviceToken;

		public string DeviceToken
		{
			get
			{
				return _deviceToken;
			}
			set
			{
				SetPropertyChanged(ref _deviceToken, value);
			}
		}

		string _devicePlatform;

		public string DevicePlatform
		{
			get
			{
				return _devicePlatform;
			}
			set
			{
				SetPropertyChanged(ref _devicePlatform, value);
			}
		}

		string _notificationRegistrationId;

		public string NotificationRegistrationId
		{
			get
			{
				return _notificationRegistrationId;
			}
			set
			{
				SetPropertyChanged(ref _notificationRegistrationId, value);
			}
		}

		string _profileImageUrl;

		public string ProfileImageUrl
		{
			get
			{
				return _profileImageUrl;
			}
			set
			{
				SetPropertyChanged(ref _profileImageUrl, value);
			}
		}

		public void RefreshMemberships()
		{
			_memberships.Clear();
			//TODO Error here when deleting an existing league
			DataManager.Instance.Memberships.Values.Where(m => m.AthleteId == Id).OrderBy(l => l.League?.Name).ToList().ForEach(_memberships.Add);
		}

		[JsonIgnore]
		public List<League> Leagues
		{
			get
			{
				return Memberships.Select(m => m.League).ToList();
			}
		}

		//		public void RefreshChallenges()
		//		{
		//			if(IncomingChallenges == null)
		//				IncomingChallenges = new List<Challenge>();
		//
		//			if(OutgoingChallenges == null)
		//				OutgoingChallenges = new List<Challenge>();
		//
		//			IncomingChallenges.Clear();
		//			DataManager.Instance.Challenges.Values.Where(m => m.ChallengeeAthleteId == Id).ToList().ForEach(IncomingChallenges.Add);
		//
		//			OutgoingChallenges.Clear();
		//			DataManager.Instance.Challenges.Values.Where(m => m.ChallengerAthleteId == Id).ToList().ForEach(OutgoingChallenges.Add);
		//		}

		//		public Challenge GetPreviousChallengeForLeague(League league)
		//		{
		//			return AllChallenges.Where(c => c.LeagueId == league.Id && c.IsCompleted).OrderByDescending(c => c.DateCompleted).FirstOrDefault();
		//		}
		//
		//		public Challenge GetOngoingChallengeForLeague(League league)
		//		{
		//			return AllChallenges.FirstOrDefault(c => c.LeagueId == league.Id && !c.IsCompleted);
		//		}
	}
}