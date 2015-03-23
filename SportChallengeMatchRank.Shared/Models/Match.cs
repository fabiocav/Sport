using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SportChallengeMatchRank.Shared
{
	public class Match : BaseModel
	{
		List<string> _memberIds;
		public const string MembershipIdsPropertyName = "MembershipIds";

		public List<string> MembershipIds
		{
			get
			{
				return _memberIds;
			}
			set
			{
				SetProperty(ref _memberIds, value, MembershipIdsPropertyName);
			}
		}

		List<Membership> _members;
		public const string MembersPropertyName = "Members";

		public List<Membership> Members
		{
			get
			{
				return _members;
			}
			set
			{
				SetProperty(ref _members, value, MembersPropertyName);
			}
		}

		DateTime _startDate;
		public const string StartDatePropertyName = "StartDate";

		public DateTime StartDate
		{
			get
			{
				return _startDate;
			}
			set
			{
				SetProperty(ref _startDate, value, StartDatePropertyName);
			}
		}

		DateTime _endDate;
		public const string EndDatePropertyName = "EndDate";

		public DateTime EndDate
		{
			get
			{
				return _endDate;
			}
			set
			{
				SetProperty(ref _endDate, value, EndDatePropertyName);
			}
		}

		List<GameResult> _gameResults;
		public const string GameResultsPropertyName = "GameResults";

		public List<GameResult> GameResults
		{
			get
			{
				return _gameResults;
			}
			set
			{
				SetProperty(ref _gameResults, value, GameResultsPropertyName);
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

	}
}