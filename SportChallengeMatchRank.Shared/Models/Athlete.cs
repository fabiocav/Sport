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

		string _email;
		public const string EmailPropertyName = "Email";

		public string Email
		{
			get
			{
				return _email;
			}
			set
			{
				SetProperty(ref _email, value, EmailPropertyName);
			}
		}

		string _authenticationId;
		public const string AuthenticationIdPropertyName = "AuthenticationId";

		public string AuthenticationId
		{
			get
			{
				return _authenticationId;
			}
			set
			{
				SetProperty(ref _authenticationId, value, AuthenticationIdPropertyName);
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
			DataManager.Instance.Memberships.Values.Where(m => m.AthleteId == Id).ToList().ForEach(_memberships.Add);
		}
	}
}