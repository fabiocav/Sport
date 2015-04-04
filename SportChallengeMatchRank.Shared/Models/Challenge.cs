﻿using System;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace SportChallengeMatchRank.Shared
{
	public class Challenge : BaseModel
	{
		public Challenge()
		{
			GameResults = new List<GameResult>();
		}

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

		public Athlete WinningAthlete
		{
			get
			{
				if(GameResults.Count != League.MatchGameCount)
					return null;

				int a = this.GetChallengerWinningGames().Length;
				int b = this.GetChallengeeWinningGames().Length;

				if(a > b)
					return ChallengerAthlete;

				return b > a ? ChallengeeAthlete : null;
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
				OnPropertyChanged("Summary");
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
				OnPropertyChanged("ChallengerAthlete");
				OnPropertyChanged("Summary");
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
				OnPropertyChanged("ChallengeeAthlete");
				OnPropertyChanged("Summary");
			}
		}

		DateTimeOffset? _dateCompleted;
		public const string DateCompletedPropertyName = "DateCompleted";

		public DateTimeOffset? DateCompleted
		{
			get
			{
				return _dateCompleted;
			}
			set
			{
				SetProperty(ref _dateCompleted, value, DateCompletedPropertyName);
				OnPropertyChanged("IsCompleted");
			}
		}

		DateTimeOffset? _dateAccepted;
		public const string DateAcceptedPropertyName = "DateAccepted";

		public DateTimeOffset? DateAccepted
		{
			get
			{
				return _dateAccepted;
			}
			set
			{
				SetProperty(ref _dateAccepted, value, DateAcceptedPropertyName);
				OnPropertyChanged("IsAccepted");
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
				OnPropertyChanged("Summary");
			}
		}

		public bool IsAccepted
		{
			get
			{
				return DateAccepted.HasValue;
			}
		}

		public bool IsCompleted
		{
			get
			{
				return DateCompleted.HasValue;
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

		public string Summary
		{
			get
			{
				if(League == null || ChallengerAthlete == null || ChallengeeAthlete == null)
					return null;

				return "{0} presents {1} vs {2} on {3}".Fmt(League.Name, ChallengerAthlete.Name, ChallengeeAthlete.Name, ProposedTime.ToLocalTime().LocalDateTime.ToString("g"));
			}
		}

		List<GameResult> gameResults;
		public const string GameResultsPropertyName = "GameResults";

		public List<GameResult> GameResults
		{
			get
			{
				return gameResults;
			}
			set
			{
				SetProperty(ref gameResults, value, GameResultsPropertyName);
			}
		}
	}
}