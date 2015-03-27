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
		public static Athlete ToAthlete(this AthleteDto dto)
		{
			return new Athlete
			{
				Name = dto.Name,
				Id = dto.Id,
				Email = dto.Email,
				IsAdmin = dto.IsAdmin,
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
	}
}