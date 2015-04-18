using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace SportChallengeMatchRank.Shared
{
	public class Challenge : BaseModel
	{
		public Challenge()
		{
			MatchResult = new List<GameResult>();
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

		[JsonIgnore]
		public Athlete WinningAthlete
		{
			get
			{
				if(MatchResult.Count != League.MatchGameCount)
					return null;

				int a = this.GetChallengerWinningGames().Length;
				int b = this.GetChallengeeWinningGames().Length;

				if(a > b)
					return ChallengerAthlete;

				return b > a ? ChallengeeAthlete : null;
			}
		}

		string _leagueId;

		public string LeagueId
		{
			get
			{
				return _leagueId;
			}
			set
			{
				ProcPropertyChanged(ref _leagueId, value);
				SetPropertyChanged("League");
				SetPropertyChanged("Summary");
			}
		}

		string _challengerAthleteId;

		public string ChallengerAthleteId
		{
			get
			{
				return _challengerAthleteId;
			}
			set
			{
				ProcPropertyChanged(ref _challengerAthleteId, value);
				SetPropertyChanged("ChallengerAthlete");
				SetPropertyChanged("Summary");
			}
		}

		string _challengeeAthleteId;

		public string ChallengeeAthleteId
		{
			get
			{
				return _challengeeAthleteId;
			}
			set
			{
				ProcPropertyChanged(ref _challengeeAthleteId, value);
				SetPropertyChanged("ChallengeeAthlete");
				SetPropertyChanged("Summary");
			}
		}

		DateTimeOffset? _dateCompleted;

		public DateTimeOffset? DateCompleted
		{
			get
			{
				return _dateCompleted;
			}
			set
			{
				ProcPropertyChanged(ref _dateCompleted, value);
				SetPropertyChanged("IsCompleted");
			}
		}

		DateTimeOffset? _dateAccepted;

		public DateTimeOffset? DateAccepted
		{
			get
			{
				return _dateAccepted;
			}
			set
			{
				ProcPropertyChanged(ref _dateAccepted, value);
				SetPropertyChanged("IsAccepted");
			}
		}

		DateTimeOffset _proposedTime;

		public DateTimeOffset ProposedTime
		{
			get
			{
				return _proposedTime;
			}
			set
			{
				ProcPropertyChanged(ref _proposedTime, value);
				SetPropertyChanged("Summary");
			}
		}

		[JsonIgnore]
		public bool IsAccepted
		{
			get
			{
				return DateAccepted.HasValue;
			}
		}

		[JsonIgnore]
		public bool IsCompleted
		{
			get
			{
				return DateCompleted.HasValue;
			}
		}

		string _customMessage;

		public string CustomMessage
		{
			get
			{
				return _customMessage;
			}
			set
			{
				ProcPropertyChanged(ref _customMessage, value);
			}
		}

		[JsonIgnore]
		public string Summary
		{
			get
			{
				if(League == null || ChallengerAthlete == null || ChallengeeAthlete == null)
					return null;

				return "{0} {1} vs {2} on {3}".Fmt(League.Name, ChallengerAthlete.Name, ChallengeeAthlete.Name, ProposedTime.ToLocalTime().LocalDateTime.ToString("d"));
			}
		}

		List<GameResult> _matchResult;

		public List<GameResult> MatchResult
		{
			get
			{
				return _matchResult;
			}
			set
			{
				ProcPropertyChanged(ref _matchResult, value);
			}
		}
	}
}