﻿using System;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace SportChallengeMatchRank.Shared
{
	public class Challenge : BaseModel
	{
		[JsonIgnore]
		public Athlete ChallengeeAthlete
		{
			get
			{
				return ChallengeeAthleteId == null ? null : DataManager.Instance.Athletes.Get(ChallengeeAthleteId);
			}
		}

		[JsonIgnore]
		public Athlete ChallengerAthlete
		{
			get
			{
				return ChallengerAthleteId == null ? null : DataManager.Instance.Athletes.Get(ChallengerAthleteId);
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

		string _challengerAthleteId;
		public const string ChallengerAthleteIdPropertyName = "ChallengerAthleteId";

		public string ChallengerAthleteId
		{
			get
			{
				return _challengerAthleteId;
			}
			set
			{
				SetProperty(ref _challengerAthleteId, value, ChallengerAthleteIdPropertyName);
			}
		}

		string _challengeeAthleteId;
		public const string ChallengeeAthleteIdPropertyName = "ChallengeeAthleteId";

		public string ChallengeeAthleteId
		{
			get
			{
				return _challengeeAthleteId;
			}
			set
			{
				SetProperty(ref _challengeeAthleteId, value, ChallengeeAthleteIdPropertyName);
			}
		}

		DateTimeOffset _proposedTime;
		public const string ProposedTimePropertyName = "ProposedTime";

		public DateTimeOffset ProposedTime
		{
			get
			{
				return _proposedTime;
			}
			set
			{
				SetProperty(ref _proposedTime, value, ProposedTimePropertyName);
			}
		}

		bool _isAccepted;
		public const string IsAcceptedPropertyName = "IsAccepted";

		public bool IsAccepted
		{
			get
			{
				return _isAccepted;
			}
			set
			{
				SetProperty(ref _isAccepted, value, IsAcceptedPropertyName);
			}
		}

		string _customMessage;
		public const string CustomMessagePropertyName = "CustomMessage";

		public string CustomMessage
		{
			get
			{
				return _customMessage;
			}
			set
			{
				SetProperty(ref _customMessage, value, CustomMessagePropertyName);
			}
		}
	}
}