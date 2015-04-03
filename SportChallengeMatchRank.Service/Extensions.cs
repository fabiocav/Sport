using Newtonsoft.Json;
using SportChallengeMatchRank;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SportChallengeMatchRank.Service
{
	public static class Extensions
	{
		public static void Shuffle<T>(this IList<T> list)
		{
			Random rng = new Random();
			int n = list.Count;
			while(n > 1)
			{
				n--;
				int k = rng.Next(n + 1);
				T value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
		}

		public static GameResult ToGameResult(this GameResultDto dto)
		{
			return new GameResult
			{
				Id = dto.Id,
				ChallengeId = dto.ChallengeId,
				ChallengeeScore = dto.ChallengeeScore,
				ChallengerScore = dto.ChallengerScore,
				Index = dto.Index
			};
		}

		public static ChallengeDto ToChallengeDto(this Challenge c)
		{
			return new ChallengeDto
			{
				Id = c.Id,
				ChallengerAthleteId = c.ChallengerAthleteId,
				ChallengeeAthleteId = c.ChallengeeAthleteId,
				LeagueId = c.LeagueId,
				DateCreated = c.CreatedAt,
				ProposedTime = c.ProposedTime,
				DateAccepted = c.DateAccepted,
				DateCompleted = c.DateCompleted,
				CustomMessage = c.CustomMessage,
			};
		}

		public static Challenge ToChallenge(this ChallengeDto dto)
		{
			return new Challenge
			{
				Id = dto.Id,
				ChallengerAthleteId = dto.ChallengerAthleteId,
				ChallengeeAthleteId = dto.ChallengeeAthleteId,
				LeagueId = dto.LeagueId,
				ProposedTime = dto.ProposedTime,
				DateAccepted = dto.DateAccepted,
				DateCompleted = dto.DateCompleted,
				CustomMessage= dto.CustomMessage
			};
		}

		public static Athlete ToAthlete(this AthleteDto dto)
		{
			return new Athlete
			{
				Name = dto.Name,
				Id = dto.Id,
				Email = dto.Email,
				IsAdmin = dto.IsAdmin,
				DeviceToken = dto.DeviceToken,
				DevicePlatform = dto.DevicePlatform,
				AuthenticationId = dto.AuthenticationId
			};
		}

		public static League ToLeague(this LeagueDto dto)
		{
			return new League
			{
				Id = dto.Id,
				Name = dto.Name,
				Description = dto.Description,
				Sport = dto.Sport,
				IsEnabled = dto.IsEnabled,
				StartDate = dto.StartDate,
				EndDate = dto.EndDate,
				Season = dto.Season,
				MaxChallengeRange = dto.MaxChallengeRange,
				HasStarted = dto.HasStarted,
				ImageUrl = dto.ImageUrl,
				CreatedByAthleteId = dto.CreatedByAthleteId,
				IsAcceptingMembers = dto.IsAcceptingMembers
			};
		}

		public static Membership ToMember(this MembershipDto dto)
		{
			return new Membership
			{
				Id = dto.Id,
				CurrentRank = dto.CurrentRank,
				AthleteId = dto.AthleteId,
				LeagueId = dto.LeagueId,
			};
		}

		public static AthleteDto ToAthleteDto(this Athlete athlete)
		{
			return new AthleteDto
			{
				Name = athlete.Name,
				Id = athlete.Id,
				Email = athlete.Email,
				AuthenticationId = athlete.AuthenticationId
			};
		}

		public static string Fmt(this string s, params object[] args)
		{
			return string.Format(s, args);
		}
	}
}