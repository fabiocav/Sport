using System;
using System.Linq;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using Xamarin.Forms;
using Newtonsoft.Json;

namespace SportChallengeMatchRank.Shared
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

		#region Push Notifications

		async public Task UpdateAthleteRegistrationForPush()
		{
			if(App.CurrentAthlete == null || App.CurrentAthlete.Id == null || App.DeviceToken == null)
				return;

			var tags = new List<string> {
					App.CurrentAthlete.Id,
					"All",
			};

			App.CurrentAthlete.Memberships.Select(m => m.LeagueId).ToList().ForEach(tags.Add);
			tags.ForEach(Console.WriteLine);
			var push = AzureService.Instance.Client.GetPush();
			//push.RegisterTemplateAsync
			await push.RegisterNativeAsync(App.DeviceToken, tags);

			if(App.CurrentAthlete.Id != null)
			{
				App.CurrentAthlete.DeviceToken = App.DeviceToken;
				await AzureService.Instance.SaveAthlete(App.CurrentAthlete);
			}
		}

		async public Task UnregisterAthleteForPush()
		{
			var push = AzureService.Instance.Client.GetPush();
			await push.UnregisterNativeAsync();
		}

		#endregion

		#region League

		async public Task<List<League>> GetAllLeagues()
		{
			DataManager.Instance.Leagues.Clear();
			var list = await Client.GetTable<League>().OrderBy(l => l.Name).ToListAsync();
			list.ForEach(l => DataManager.Instance.Leagues.AddOrUpdate(l));
			return list;
		}

		async public Task GetAllAthletesByLeague(League league)
		{
			var memberships = await Client.GetTable<Membership>().Where(m => m.LeagueId == league.Id).OrderBy(m => m.CurrentRank).ToListAsync();
			var athleteIds = memberships.Where(m => !DataManager.Instance.Athletes.ContainsKey(m.AthleteId)).Select(m => m.AthleteId).ToList();
			var athletes = new List<Athlete>();

			athletes = await Client.GetTable<Athlete>().Where(a => athleteIds.Contains(a.Id)).OrderBy(a => a.Name).ToListAsync();

			foreach(var m in DataManager.Instance.Memberships.Values.Where(m => m.LeagueId == league.Id).ToList())
			{
				Membership mem;
				DataManager.Instance.Memberships.TryRemove(m.Id, out mem);
			}

			foreach(var m in memberships)
			{
				var athlete = athletes.SingleOrDefault(a => a.Id == m.AthleteId);
				athlete = athlete ?? DataManager.Instance.Athletes.Get(m.AthleteId);

				if(athlete == null)
				{
					await DeleteMembership(m.Id);
					continue;
				}

				DataManager.Instance.Memberships.AddOrUpdate(m);
				DataManager.Instance.Athletes.AddOrUpdate(athlete);
				m.OnPropertyChanged("Athlete");
			}

			DataManager.Instance.Athletes.Values.ToList().ForEach(a => a.RefreshMemberships());
			DataManager.Instance.Leagues.Values.ToList().ForEach(l => l.RefreshMemberships());
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
				a = a ?? await Client.GetTable<League>().LookupAsync(id);
				DataManager.Instance.Leagues.AddOrUpdate(a);
				return a;
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

		async public Task<SaveLeagueResult> SaveLeague(League league)
		{
			try
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
				return SaveLeagueResult.OK;
			}
			catch(MobileServiceConflictException)
			{
				return SaveLeagueResult.Conflict;
			}
			catch(Exception e)
			{
				Console.WriteLine(e);
				return SaveLeagueResult.Failed;
			}
		}

		async public Task DeleteLeague(string id)
		{
			League l;
			try
			{
				await Client.GetTable<League>().DeleteAsync(new League {
						Id = id
					});
				DataManager.Instance.Leagues.TryRemove(id, out l);
			}
			catch(HttpRequestException hre)
			{
				if(hre.Message.ContainsNoCase("not found"))
				{
					DataManager.Instance.Leagues.TryRemove(id, out l);
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
			list.ForEach(a => DataManager.Instance.Athletes.AddOrUpdate(a));
			return list;
		}

		async public Task<Athlete> GetAthleteByEmail(string email)
		{
			Console.WriteLine("Getting athlete by email" + email);
			try
			{
				var list = await Client.GetTable<Athlete>().Where(a => a.Email == email).Take(1).ToListAsync();
				var athlete = list.FirstOrDefault();

				if(athlete != null)
					DataManager.Instance.Athletes.AddOrUpdate(athlete);

				return athlete;
			}
			catch(Exception)
			{
				NotifyOfFailure("Unable to connect to service");
				throw;
				//Console.WriteLine(e);
			}
		}

		async public Task<Athlete> GetAthleteByAuthUserId(string authUserid)
		{
			Console.WriteLine("Getting athlete by id" + authUserid);
			try
			{
				var list = await Client.GetTable<Athlete>().Where(a => a.AuthenticationId == authUserid).Take(1).ToListAsync();
				var athlete = list.FirstOrDefault();

				if(athlete != null)
					DataManager.Instance.Athletes.AddOrUpdate(athlete);

				return athlete;				
			}
			catch(Exception)
			{
				NotifyOfFailure("Unable to connect to service");
				throw;
				//Console.WriteLine(e);
			}
		}

		async public Task<Athlete> GetAthleteById(string id)
		{
			Athlete a;
			DataManager.Instance.Athletes.TryGetValue(id, out a);
			a = a ?? await Client.GetTable<Athlete>().LookupAsync(id);

			if(a != null)
				DataManager.Instance.Athletes.AddOrUpdate(a);

			return a;				
		}

		async public Task GetAllLeaguesByAthlete(Athlete athlete)
		{
			var memberships = await Client.GetTable<Membership>().Where(m => m.AthleteId == athlete.Id).OrderBy(m => m.CurrentRank).ToListAsync();
			var leagueIds = memberships.Where(m => !DataManager.Instance.Leagues.ContainsKey(m.LeagueId)).Select(m => m.LeagueId).ToList();
			var leagues = new List<League>();

			leagues = await Client.GetTable<League>().Where(l => leagueIds.Contains(l.Id)).OrderBy(l => l.Name).ToListAsync();

			foreach(var m in DataManager.Instance.Memberships.Values.Where(m => m.AthleteId == athlete.Id).ToList())
			{
				Membership mem;
				DataManager.Instance.Memberships.TryRemove(m.Id, out mem);
			}

			foreach(var m in memberships)
			{
				var league = leagues.SingleOrDefault(l => l.Id == m.LeagueId);
				league = league ?? DataManager.Instance.Leagues.Get(m.LeagueId);

				if(league == null)
				{
					await DeleteMembership(m.Id);
				}
				
				DataManager.Instance.Memberships.AddOrUpdate(m);
				DataManager.Instance.Leagues.AddOrUpdate(league);
				m.OnPropertyChanged("League");
			}

			DataManager.Instance.Athletes.Values.ToList().ForEach(a => a.RefreshMemberships());
			DataManager.Instance.Leagues.Values.ToList().ForEach(l => l.RefreshMemberships());
		}

		async public Task SaveAthlete(Athlete athlete)
		{
			try
			{
				if(athlete.Id == null)
				{
					if(athlete.Email == "rob.derosa@xamarin.com")
						athlete.IsAdmin = true;

					//Stopped here - need to handle conflicts
					athlete.DevicePlatform = Xamarin.Forms.Device.OS.ToString();
					await Client.GetTable<Athlete>().InsertAsync(athlete);
				}
				else
				{
					await Client.GetTable<Athlete>().UpdateAsync(athlete);
				}

				DataManager.Instance.Athletes.AddOrUpdate(athlete);
			}
			catch(MobileServiceConflictException conflict)
			{
				
			}
			catch(Exception e)
			{
					
			}
		}

		async public Task DeleteAthlete(string id)
		{
			Athlete a;
			try
			{
				await Client.GetTable<Athlete>().DeleteAsync(new Athlete {
						Id = id
					});

				DataManager.Instance.Athletes.TryRemove(id, out a);
				await AzureService.Instance.UnregisterAthleteForPush();
			}
			catch(HttpRequestException hre)
			{
				if(hre.Message.ContainsNoCase("not found"))
				{
					DataManager.Instance.Athletes.TryRemove(id, out a);
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

		async public Task<DateTime?> StartLeague(string id)
		{
			try
			{
				var qs = new Dictionary<string, string>();
				qs.Add("id", id);
				var dateTime = await Client.InvokeApiAsync("startLeague", null, HttpMethod.Post, qs);
				return (DateTime)dateTime.Root;
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
			membership.LocalRefresh();

			await AzureService.Instance.UpdateAthleteRegistrationForPush();
		}

		async public Task DeleteMembership(string id)
		{
			Membership m;
			try
			{
				await Client.GetTable<Membership>().DeleteAsync(new Membership {
						Id = id
					});

				DataManager.Instance.Memberships.TryRemove(id, out m);
				m.LocalRefresh();
				await AzureService.Instance.UpdateAthleteRegistrationForPush();
			}
			catch(HttpRequestException hre)
			{
				if(hre.Message.ContainsNoCase("not found"))
				{
					DataManager.Instance.Memberships.TryRemove(id, out m);
				}
			}
			catch(Exception e)
			{
				Console.WriteLine(e);
			}
		}

		#endregion

		#region Challenge

		async public Task SaveChallenge(Challenge challenge)
		{
			if(challenge.Id == null)
			{
				await Client.GetTable<Challenge>().InsertAsync(challenge);
			}
			else
			{
				await Client.GetTable<Challenge>().UpdateAsync(challenge);
			}

			DataManager.Instance.Challenges.AddOrUpdate(challenge);

			if(challenge.ChallengeeAthlete != null)
				challenge.ChallengeeAthlete.RefreshChallenges();

			if(challenge.ChallengerAthlete != null)
				challenge.ChallengerAthlete.RefreshChallenges();
		}

		async public Task DeleteChallenge(string id)
		{
			Challenge m;
			try
			{
				await Client.GetTable<Challenge>().DeleteAsync(new Challenge {
						Id = id
					});

				DataManager.Instance.Challenges.TryRemove(id, out m);
			}
			catch(HttpRequestException hre)
			{
				if(hre.Message.ContainsNoCase("not found"))
				{
					DataManager.Instance.Challenges.TryRemove(id, out m);
				}
			}
			catch(Exception e)
			{
				Console.WriteLine(e);
			}
		}

		async public Task GetAllChallengesByAthlete(Athlete athlete)
		{
			DataManager.Instance.Challenges.Clear();
			var challenges = await Client.GetTable<Challenge>().Where(c => c.ChallengeeAthleteId == athlete.Id || c.ChallengerAthleteId == athlete.Id).OrderBy(c => c.DateCreated).ToListAsync();
			challenges.ForEach(DataManager.Instance.Challenges.AddOrUpdate);
			athlete.RefreshChallenges();
		}

		async public Task AcceptChallenge(Challenge challenge)
		{
			try
			{
				var qs = new Dictionary<string, string>();
				qs.Add("challengeId", challenge.Id);
				var token = await Client.InvokeApiAsync("acceptChallenge", null, HttpMethod.Post, qs);
				var acceptedChallenge = JsonConvert.DeserializeObject<Challenge>(token.ToString());
				if(acceptedChallenge != null)
				{
					challenge.IsAccepted = acceptedChallenge.IsAccepted;
					challenge.DateAccepted = acceptedChallenge.DateAccepted;
				}
			}
			catch(Exception e)
			{
				Console.WriteLine(e);
			}
		}

		async public Task DeclineChallenge(string challengeId)
		{
			await DeleteChallenge(challengeId);
		}

		#endregion

		void NotifyOfFailure(string message)
		{
			Console.WriteLine("Notification sent for ServiceCallFailed");
			MessagingCenter.Send<AzureService, string>(this, "ServiceCallFailed", message);
		}
	}

	public enum SaveLeagueResult
	{
		None,
		OK,
		Failed,
		Conflict
	}
}