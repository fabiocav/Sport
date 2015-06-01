using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Windows.Input;
using System.Linq;

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
				SetPropertyChanged(ref _leagueId, value);
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
				SetPropertyChanged(ref _challengerAthleteId, value);
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
				SetPropertyChanged(ref _challengeeAthleteId, value);
				SetPropertyChanged("ChallengeeAthlete");
				SetPropertyChanged("Summary");
			}
		}

		int _height = 30;

		public int Height
		{
			get
			{
				return _height;
			}
			set
			{
				SetPropertyChanged(ref _height, value);
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
				SetPropertyChanged(ref _dateCompleted, value);
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
				SetPropertyChanged(ref _dateAccepted, value);
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
				SetPropertyChanged(ref _proposedTime, value);
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
				SetPropertyChanged(ref _customMessage, value);
			}
		}

		[JsonIgnore]
		public string ProposedTimeString
		{
			get
			{
				var date = ProposedTime.ToLocalTime().LocalDateTime;
				return "{0} at {1}".Fmt(date.ToString("dddd, M"), date.ToString("t"), League.Name);
			}
		}

		[JsonIgnore]
		public string BattleFor
		{
			get
			{
				if(League == null)
					return null;
				
				var mem = League.Memberships.SingleOrDefault(m => m.AthleteId == ChallengeeAthleteId);
				var desc = mem == null ? null : "an epic battle for {0} place".Fmt((mem.CurrentRank + 1).ToOrdinal());

				if(mem == null)
					return null;

				if(ChallengerAthlete == null || ChallengeeAthlete == null)
					return desc;

				desc += " between {0} and {1}".Fmt(ChallengerAthlete.Alias, ChallengeeAthlete.Alias);
				return desc;
			}
		}

		[JsonIgnore]
		public string Summary
		{
			get
			{
				if(League == null || ChallengerAthlete == null || ChallengeeAthlete == null)
					return null;

				return "{0} vs {1}".Fmt(ChallengerAthlete.Alias, ChallengeeAthlete.Alias);
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
				SetPropertyChanged(ref _matchResult, value);
			}
		}

		public ICommand ResizeCommand
		{
			get
			{
				return new Command(() =>
				{
					Height = 100;
				});
			}
		}

		public void NotifyPropertiesChanged()
		{
			SetPropertyChanged("League");
			SetPropertyChanged("Summary");
			SetPropertyChanged("ChallengeeAthlete");
			SetPropertyChanged("ChallengerAthlete");
			SetPropertyChanged("IsCompleted");
			SetPropertyChanged("ProposedTimeString");
			SetPropertyChanged("IsAccepted");
			SetPropertyChanged("BattleFor");
		}
	}
}