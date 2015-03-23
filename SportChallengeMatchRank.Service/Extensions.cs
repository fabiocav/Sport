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
				AuthenticationId = dto.AuthenticationId
			};
		}

		public static League ToLeague(this LeagueDto dto)
		{
			return new League
			{
				Name = dto.Name,
				Id = dto.Id,
				Sport = dto.Sport,
				IsEnabled = dto.IsEnabled,
				IsAcceptingMembers = dto.IsAcceptingMembers,
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

		public static void LoadAthleteIds(this League league)
		{
			if(league != null)
			{
				//if(!string.IsNullOrWhiteSpace(league.AthleteIdsString))
				//{
				//	league.AthleteIds = JsonConvert.DeserializeObject<List<string>>(league.AthleteIdsString);
				//}
			}
		}
	}
}