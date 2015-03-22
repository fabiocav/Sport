using System;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using System.Net;
using System.Collections.Generic;
using System.Net.Http;

namespace SportRankerMatchOn.Shared
{
	public class AzureService
	{
		static AzureService _instance;

		public static AzureService Instance
		{
			get
			{
				return _instance ?? (_instance = new AzureService());
			}
		}

		public League DefaultLeague
		{
			get;
			set;
		}

		MobileServiceClient _client;

		public MobileServiceClient Client
		{
			get
			{
				if(_client == null)
				{
					_client = new MobileServiceClient(Constants.AzureDomain, Constants.AzureClientId);
					CurrentPlatform.Init();
				}

				return _client;
			}			
		}

		async public Task<Member> AddAthleteToLeague(string athleteId, string leagueId)
		{
			try
			{
				await _client.InvokeApiAsync<string[], object>("add_to_league", new[] {
						athleteId,
						leagueId,
					});
			}
			catch(MobileServiceInvalidOperationException ex)
			{
				Console.WriteLine(ex.Message);
			}
			catch(HttpRequestException ex2)
			{
				Console.WriteLine(ex2.Message);
			}

			return new Member {
				Athlete = App.CurrentAthlete,
				LeagueId = leagueId,
			};
		}

		#region League

		async public Task<List<League>> GetAllLeagues()
		{
			var list = await Client.GetTable<League>().OrderBy(l => l.Name).ToListAsync();
			return list;
		}

		async public Task<List<League>> GetAllEnabledLeagues()
		{
			var list = await Client.GetTable<League>().Where(l => l.IsEnabled).OrderBy(l => l.Name).ToListAsync();
			return list;
		}

		async public Task<League> GetLeagueByName(string name)
		{
			var list = await Client.GetTable<League>().Where(l => l.Name == name).Take(1).ToListAsync();
			DefaultLeague = list.FirstOrDefault();
			return DefaultLeague;
		}

		async public Task SaveLeague(League league)
		{
			if(league.Id == null)
			{
				await Client.GetTable<League>().InsertAsync(league);
			}
			else
			{
				await Client.GetTable<League>().UpdateAsync(league);
			}
		}

		async public Task DeleteLeague(string id)
		{
			try
			{
				await Client.GetTable<League>().DeleteAsync(new League {
						Id = id
					});
			}
			catch(Exception e)
			{
				Console.WriteLine(e);
			}
		}

		#endregion

		#region Athlete

		async public Task<List<Athlete>> GetAllAthletes()
		{
			var list = await Client.GetTable<Athlete>().OrderBy(a => a.Name).ToListAsync();
			return list;
		}

		async public Task<Athlete> GetAthleteById(string id)
		{
			try
			{
				return await Client.GetTable<Athlete>().LookupAsync(id);
			}
			catch(Exception e)
			{
				Console.WriteLine(e);
			}

			return null;
		}

		async public Task SaveAthlete(Athlete athlete)
		{
			if(athlete.Id == null)
			{
				await Client.GetTable<Athlete>().InsertAsync(athlete);
			}
			else
			{
				await Client.GetTable<Athlete>().UpdateAsync(athlete);
			}
		}

		async public Task DeleteAthlete(string id)
		{
			try
			{
				await Client.GetTable<Athlete>().DeleteAsync(new Athlete {
						Id = id
					});
			}
			catch(Exception e)
			{
				Console.WriteLine(e);
			}
		}

		#endregion

		#region Member

		async public Task<Member> GetMemberById(string id)
		{
			try
			{
				return await Client.GetTable<Member>().LookupAsync(id);
			}
			catch(Exception e)
			{
				Console.WriteLine(e);
			}

			return null;
		}

		async public Task SaveMember(Member member)
		{
			if(member.Id == null)
			{
				await Client.GetTable<Member>().InsertAsync(member);
			}
			else
			{
				await Client.GetTable<Member>().UpdateAsync(member);
			}
		}

		async public Task DeleteMember(string id)
		{
			try
			{
				await Client.GetTable<Member>().DeleteAsync(new Member {
						Id = id
					});
			}
			catch(Exception e)
			{
				Console.WriteLine(e);
			}
		}

		#endregion
	}
}

