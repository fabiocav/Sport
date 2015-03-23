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

		void Initialize()
		{
			Memberships = new ObservableCollection<Membership>();

			IsAcceptingMembers = true;
			IsEnabled = true;
		}

		public List<string> MembershipIds
		{
			get;
			set;
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

		public void RefreshMemberships()
		{
			_memberships.Clear();
			DataManager.Instance.Memberships.Values.Where(m => m.LeagueId == Id).ToList().ForEach(_memberships.Add);
		}
	}

	public enum Sports
	{
		TableTennis,
		Foosball,
	}
}