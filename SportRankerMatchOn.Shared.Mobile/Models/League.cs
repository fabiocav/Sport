using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SportRankerMatchOn.Shared
{
	public class League : BaseModel
	{
		public League()
		{
			MemberIds = new List<string>();
		}

		string _name;
		public const string NamePropertyName = "Name";

		[JsonProperty("name")]
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

		[JsonProperty("sport")]
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

		[JsonProperty("is-enabled")]
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

		bool _isAcceptingNewMembers;
		public const string IsAcceptingNewMembersPropertyName = "IsAcceptingNewMembers";

		[JsonProperty("is-accepting-new-members")]
		public bool IsAcceptingNewMembers
		{
			get
			{
				return _isAcceptingNewMembers;
			}
			set
			{
				SetProperty(ref _isAcceptingNewMembers, value, IsAcceptingNewMembersPropertyName);
			}
		}

		int _season;
		public const string SeasonPropertyName = "Season";

		[JsonProperty("season")]
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

		List<string> _memberIds;
		public const string MemberIdsPropertyName = "MemberIds";

		[JsonProperty("member-ids")]
		public List<string> MemberIds
		{
			get
			{
				return _memberIds;
			}
			set
			{
				SetProperty(ref _memberIds, value, MemberIdsPropertyName);
			}
		}
	}

	public enum Sports
	{
		TableTennis,
		Foosball,
	}
}