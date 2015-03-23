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
		#region Properties

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
					HttpClientHandler handler = new HttpClientHandler();

					#if __IOS__
					handler = new HttpClientHandler {
						Proxy = CoreFoundation.CFNetwork.GetDefaultProxy(),
						UseProxy = true,
					};
					#endif

					_client = new MobileServiceClient(Constants.AzureDomain, Constants.AzureClientId, new HttpMessageHandler[] {
							handler
						});
					CurrentPlatform.Init();
				}

				return _client;
			}			
		}

		#endregion

		#region League

		async public Task<List<League>> GetAllLeagues()
		{
			DataManager.Instance.Leagues.Clear();
			var list = await Client.GetTable<League>().OrderBy(l => l.Name).ToListAsync();
			return list;
		}

		async public Task GetAllAthletesByLeague(League league)
		{
			var memberships = await Client.GetTable<Membership>().Where(m => m.LeagueId == league.Id).OrderBy(m => m.CurrentRank).ToListAsync();
			var memberIds = memberships.Where(m => !DataManager.Instance.Leagues.ContainsKey(m.AthleteId)).Select(m => m.AthleteId).ToList();
			var athletes = await Client.GetTable<Athlete>().Where(a => memberIds.Contains(a.Id)).OrderBy(a => a.Name).ToListAsync();

			league.Memberships.Clear();
			foreach(var m in memberships)
			{
				m.Athlete = athletes.SingleOrDefault(a => a.Id == m.AthleteId);
				m.Athlete = m.Athlete ?? DataManager.Instance.Athletes[m.AthleteId];

				if(m.Athlete == null)
				{
					await DeleteMembership(m.Id);
				}

				league.Memberships.Add(m);
				DataManager.Instance.Memberships.AddOrUpdate(m);
				DataManager.Instance.Athletes.AddOrUpdate(m.Athlete);
			}
		}

		async public Task<List<League>> GetAllEnabledLeagues()
		{
			var list = await Client.GetTable<League>().Where(l => l.IsEnabled).OrderBy(l => l.Name).ToListAsync();
			return list;
		}

		async public Task<League> GetLeagueById(string id)
		{
			try
			{
				League a;
				DataManager.Instance.Leagues.TryGetValue(id, out a);
				return a ?? await Client.GetTable<League>().LookupAsync(id);
			}
			catch(Exception e)
			{
				Console.WriteLine(e);
			}

			return null;
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
			DataManager.Instance.Leagues.AddOrUpdate(league);
		}

		async public Task DeleteLeague(string id)
		{
			try
			{
				await Client.GetTable<League>().DeleteAsync(new League {
						Id = id
					});
				DataManager.Instance.Leagues.Remove(id);
			}
			catch(HttpRequestException hre)
			{
				if(hre.Message.ContainsNoCase("not found"))
				{
					DataManager.Instance.Leagues.Remove(id);
				}
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
			DataManager.Instance.Athletes.Clear();
			var list = await Client.GetTable<Athlete>().OrderBy(a => a.Name).ToListAsync();
			return list;
		}

		async public Task<Athlete> GetAthleteById(string id)
		{
			try
			{
				Athlete a;
				DataManager.Instance.Athletes.TryGetValue(id, out a);
				return a ?? await Client.GetTable<Athlete>().LookupAsync(id);
			}
			catch(Exception e)
			{
				Console.WriteLine(e);
			}

			return null;
		}

		async public Task GetAllLeaguesByAthlete(Athlete athlete)
		{
			var memberships = await Client.GetTable<Membership>().Where(m => m.AthleteId == athlete.Id).OrderBy(m => m.CurrentRank).ToListAsync();
			var memberIds = memberships.Where(m => !DataManager.Instance.Leagues.ContainsKey(m.LeagueId)).Select(m => m.LeagueId).ToList();
			var leagues = await Client.GetTable<League>().Where(l => memberIds.Contains(l.Id)).OrderBy(l => l.Name).ToListAsync();

			athlete.Memberships.Clear();
			foreach(var m in memberships)
			{
				m.League = leagues.SingleOrDefault(l => l.Id == m.LeagueId);
				m.League = m.League ?? DataManager.Instance.Leagues[m.LeagueId];

				if(m.League == null)
				{
					await DeleteMembership(m.Id);
				}
				
				athlete.Memberships.Add(m);
				DataManager.Instance.Memberships.AddOrUpdate(m);
				DataManager.Instance.Leagues.AddOrUpdate(m.League);
			}
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
			DataManager.Instance.Athletes.AddOrUpdate(athlete);
		}

		async public Task DeleteAthlete(string id)
		{
			try
			{
				await Client.GetTable<Athlete>().DeleteAsync(new Athlete {
						Id = id
					});
				DataManager.Instance.Athletes.Remove(id);
			}
			catch(HttpRequestException hre)
			{
				if(hre.Message.ContainsNoCase("not found"))
				{
					DataManager.Instance.Athletes.Remove(id);
				}
			}
			catch(Exception e)
			{
				Console.WriteLine(e);
			}
		}

		#endregion

		#region Membership

		async public Task GetMembershipsForLeague(League league)
		{
			var list = await Client.GetTable<Membership>().Where(m => m.LeagueId == league.Id).OrderBy(m => m.CurrentRank).ToListAsync();

			league.Memberships.Clear();
			foreach(var m in list)
			{
				league.Memberships.Add(m);
				DataManager.Instance.Memberships.AddOrUpdate(m);
			}
		}

		async public Task<Membership> GetMembershipById(string id)
		{
			try
			{
				Membership a;
				DataManager.Instance.Memberships.TryGetValue(id, out a);
				return a ?? await Client.GetTable<Membership>().LookupAsync(id);
			}
			catch(Exception e)
			{
				Console.WriteLine(e);
			}

			return null;
		}

		async public Task SaveMembership(Membership membership)
		{
			if(membership.Id == null)
			{
				await Client.GetTable<Membership>().InsertAsync(membership);
			}
			else
			{
				await Client.GetTable<Membership>().UpdateAsync(membership);
			}

			DataManager.Instance.Memberships.AddOrUpdate(membership);
		}

		async public Task DeleteMembership(string id)
		{
			try
			{
				await Client.GetTable<Membership>().DeleteAsync(new Membership {
						Id = id
					});
				DataManager.Instance.Memberships.Remove(id);
			}
			catch(HttpRequestException hre)
			{
				if(hre.Message.ContainsNoCase("not found"))
				{
					DataManager.Instance.Memberships.Remove(id);
				}
			}
			catch(Exception e)
			{
				Console.WriteLine(e);
			}
		}

		#endregion
	}
}

