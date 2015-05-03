using System;
using System.Linq;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace SportChallengeMatchRank.Shared
{
	public class InternetService
	{
		#region Properties

		static InternetService _instance;

		public static InternetService Instance
		{
			get
			{
				return _instance ?? (_instance = new InternetService());
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
					var handler = new HttpClientHandler();

					#if __IOS__

					//Use ModernHttpClient and allow traffic to be routed into Charles/Fiddler/etc
					handler = new ModernHttpClient.NativeMessageHandler() {
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

		public Task UpdateAthleteRegistrationForPush()
		{
			return new Task(() =>
			{
				if(App.CurrentAthlete == null || App.CurrentAthlete.Id == null || App.DeviceToken == null)
					return;

//				var tags = new List<string> {
//						App.CurrentAthlete.Id,
//						"All",
//				};

				App.CurrentAthlete.Memberships.Select(m => m.LeagueId).ToList().ForEach(tags.Add);
				tags.ForEach(Console.WriteLine);
				var push = InternetService.Instance.Client.GetPush();
				//push.RegisterTemplateAsync
				push.RegisterNativeAsync(App.DeviceToken, tags).Wait();

				if(App.CurrentAthlete.Id != null)
				{
					App.CurrentAthlete.DeviceToken = App.DeviceToken;
					InternetService.Instance.SaveAthlete(App.CurrentAthlete).Wait();
				}
			});
		}

		public Task UnregisterAthleteForPush()
		{
			return new Task(() =>
			{
				var push = InternetService.Instance.Client.GetPush();
				push.UnregisterNativeAsync().Wait();
			});
		}

		#endregion

		#region League

		public Task<List<League>> GetAllLeagues()
		{
			return new Task<List<League>>(() =>
			{
				DataManager.Instance.Leagues.Clear();
				var list = Client.GetTable<League>().OrderBy(l => l.Name).ToListAsync().Result;
				list.ForEach(l => DataManager.Instance.Leagues.AddOrUpdate(l));
				return list;
			});
		}

		public Task GetAllAthletesByLeague(League league)
		{
			return new Task(() =>
			{
				var memberships = Client.GetTable<Membership>().Where(m => m.LeagueId == league.Id).OrderBy(m => m.CurrentRank).ToListAsync().Result;
				var athleteIds = memberships.Where(m => !DataManager.Instance.Athletes.ContainsKey(m.AthleteId)).Select(m => m.AthleteId).ToList();
				var athletes = new List<Athlete>();

				if(athleteIds != null && athleteIds.Count > 0)
				{
					athleteIds.ForEach(Console.WriteLine);
					athletes = Client.GetTable<Athlete>().Where(a => athleteIds.Contains(a.Id)).OrderBy(a => a.Name).ToListAsync().Result;
				}

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
						DeleteMembership(m.Id).Wait();
						continue;
					}

					DataManager.Instance.Memberships.AddOrUpdate(m);
					DataManager.Instance.Athletes.AddOrUpdate(athlete);
					m.SetPropertyChanged("Athlete");
				}

				DataManager.Instance.Athletes.Values.ToList().ForEach(a => a.RefreshMemberships());
				DataManager.Instance.Leagues.Values.ToList().ForEach(l => l.RefreshMemberships());
			});
		}

		public Task<List<League>> GetAllEnabledLeagues()
		{
			return new Task<List<League>>(() =>
			{
				var list = Client.GetTable<League>().Where(l => l.IsEnabled).OrderBy(l => l.Name).ToListAsync().Result;
				return list;
			});
		}

		public Task<League> GetLeagueById(string id)
		{
			return new Task<League>(() =>
			{
				League a;
				DataManager.Instance.Leagues.TryGetValue(id, out a);
				a = a ?? Client.GetTable<League>().LookupAsync(id).Result;
				DataManager.Instance.Leagues.AddOrUpdate(a);
				return a;
			});
		}

		public Task<League> GetLeagueByName(string name)
		{
			return new Task<League>(() =>
			{
				var list = Client.GetTable<League>().Where(l => l.Name == name).Take(1).ToListAsync().Result;
				DefaultLeague = list.FirstOrDefault();
				return DefaultLeague;
			});
		}

		public Task SaveLeague(League league)
		{
			return new Task(() =>
			{
				if(league.Id == null)
				{
					Client.GetTable<League>().InsertAsync(league).Wait();
				}
				else
				{
					Client.GetTable<League>().UpdateAsync(league).Wait();
				}

				DataManager.Instance.Leagues.AddOrUpdate(league);
			});
		}

		public Task DeleteLeague(string id)
		{
			return new Task(() =>
			{
				League l;
				try
				{
					Client.GetTable<League>().DeleteAsync(new League {
						Id = id
					}).Wait();
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
			});
		}

		#endregion

		#region Athlete

		public Task<List<Athlete>> GetAllAthletes()
		{
			return new Task<List<Athlete>>(() =>
			{
				DataManager.Instance.Athletes.Clear();
				var list = Client.GetTable<Athlete>().OrderBy(a => a.Name).ToListAsync().Result;
				list.ForEach(a => DataManager.Instance.Athletes.AddOrUpdate(a));
				return list;
			});
		}

		public Task<Athlete> GetAthleteByEmail(string email)
		{
			return new Task<Athlete>(() =>
			{
				var list = Client.GetTable<Athlete>().Where(a => a.Email == email).ToListAsync().Result;
				var athlete = list.FirstOrDefault();

				if(athlete != null)
					DataManager.Instance.Athletes.AddOrUpdate(athlete);

				return athlete;
			});
		}

		public Task<Athlete> GetAthleteByAuthUserId(string authUserid)
		{
			return new Task<Athlete>(() =>
			{
				var list = Client.GetTable<Athlete>().Where(a => a.AuthenticationId == authUserid).ToListAsync().Result;
				var athlete = list.FirstOrDefault();

				if(athlete != null)
					DataManager.Instance.Athletes.AddOrUpdate(athlete);

				return athlete;
			});
		}

		public Task<Athlete> GetAthleteById(string id, bool force = false)
		{
			return new Task<Athlete>(() =>
			{
				Athlete a = null;

				if(!force)
					DataManager.Instance.Athletes.TryGetValue(id, out a);

				a = a ?? Client.GetTable<Athlete>().LookupAsync(id).Result;

				if(a != null)
					DataManager.Instance.Athletes.AddOrUpdate(a);

				return a;				
			});
		}

		public Task GetAllLeaguesByAthlete(Athlete athlete)
		{
			return new Task(() =>
			{
				var memberships = Client.GetTable<Membership>().Where(m => m.AthleteId == athlete.Id).OrderBy(m => m.CurrentRank).ToListAsync().Result;
				var leagueIds = memberships.Where(m => !DataManager.Instance.Leagues.ContainsKey(m.LeagueId)).Select(m => m.LeagueId).ToList();
				List<League> leagues;

				leagues = Client.GetTable<League>().Where(l => leagueIds.Contains(l.Id)).OrderBy(l => l.Name).ToListAsync().Result;

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
						DeleteMembership(m.Id).Wait();
					}

					DataManager.Instance.Memberships.AddOrUpdate(m);
					DataManager.Instance.Leagues.AddOrUpdate(league);
					m.SetPropertyChanged("League");
				}

				DataManager.Instance.Athletes.Values.ToList().ForEach(a => a.RefreshMemberships());
				DataManager.Instance.Leagues.Values.ToList().ForEach(l => l.RefreshMemberships());					
			});
		}

		public Task SaveAthlete(Athlete athlete)
		{
			return new Task(() =>
			{
				if(athlete.Id == null)
				{
					if(athlete.Email == "rob.derosa@xamarin.com")
						athlete.IsAdmin = true;

					//Stopped here - need to handle conflicts
					athlete.DevicePlatform = Xamarin.Forms.Device.OS.ToString();
					Client.GetTable<Athlete>().InsertAsync(athlete).Wait();
				}
				else
				{
					Client.GetTable<Athlete>().UpdateAsync(athlete).Wait();
				}

				DataManager.Instance.Athletes.AddOrUpdate(athlete);
			});
		}

		public Task DeleteAthlete(string id)
		{
			return new Task(() =>
			{
				Athlete a;
				try
				{
					Client.GetTable<Athlete>().DeleteAsync(new Athlete {
						Id = id
					}).Wait();

					DataManager.Instance.Athletes.TryRemove(id, out a);
					InternetService.Instance.UnregisterAthleteForPush().Wait();
				}
				catch(HttpRequestException hre)
				{
					if(hre.Message.ContainsNoCase("not found"))
					{
						DataManager.Instance.Athletes.TryRemove(id, out a);
					}
				}
			});
		}

		#endregion

		#region Membership

		public Task GetMembershipsForLeague(League league)
		{
			return new Task(() =>
			{
				var list = Client.GetTable<Membership>().Where(m => m.LeagueId == league.Id).OrderBy(m => m.CurrentRank).ToListAsync().Result;

				league.Memberships.Clear();
				foreach(var m in list)
				{
					league.Memberships.Add(m);
					DataManager.Instance.Memberships.AddOrUpdate(m);
				}
			});

		}

		public Task<Membership> GetMembershipById(string id)
		{
			return new Task<Membership>(() =>
			{
				Membership a;
				DataManager.Instance.Memberships.TryGetValue(id, out a);
				return a ?? Client.GetTable<Membership>().LookupAsync(id).Result;
			});
		}

		public Task<DateTime?> StartLeague(string id)
		{
			return new Task<DateTime?>(() =>
			{
				var qs = new Dictionary<string, string>();
				qs.Add("id", id);
				var dateTime = Client.InvokeApiAsync("startLeague", null, HttpMethod.Post, qs).Result;
				return (DateTime)dateTime.Root;
			});
		}

		public Task SaveMembership(Membership membership)
		{
			return new Task(() =>
			{
				if(membership.Id == null)
				{
					Client.GetTable<Membership>().InsertAsync(membership).Wait();
				}
				else
				{
					Client.GetTable<Membership>().UpdateAsync(membership).Wait();
				}

				DataManager.Instance.Memberships.AddOrUpdate(membership);
				membership.LocalRefresh();

				var task = InternetService.Instance.UpdateAthleteRegistrationForPush();
				task.Start();
				task.Wait();
			});
		}

		public Task DeleteMembership(string id)
		{
			return new Task(() =>
			{
				Membership m;
				try
				{
					Client.GetTable<Membership>().DeleteAsync(new Membership {
						Id = id
					}).Wait();

					DataManager.Instance.Memberships.TryRemove(id, out m);
					m.LocalRefresh();

					var challenges = DataManager.Instance.Challenges.Values.Where(c => c.LeagueId == m.LeagueId
					                 && c.ChallengerAthleteId == m.AthleteId
					                 || c.ChallengeeAthleteId == m.AthleteId).ToList();

					Challenge ch;
					challenges.ForEach(c => DataManager.Instance.Challenges.TryRemove(c.Id, out ch));

					if(m.Athlete != null)
						m.Athlete.RefreshChallenges();

					var task = InternetService.Instance.UpdateAthleteRegistrationForPush();
					task.Start();
					task.Wait();
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
			});			
		}

		#endregion

		#region Challenge

		public Task SaveChallenge(Challenge challenge)
		{
			return new Task(() =>
			{
				if(challenge.Id == null)
				{
					Client.GetTable<Challenge>().InsertAsync(challenge).Wait();
				}
				else
				{
					Client.GetTable<Challenge>().UpdateAsync(challenge).Wait();
				}

				DataManager.Instance.Challenges.AddOrUpdate(challenge);

				if(challenge.ChallengeeAthlete != null)
					challenge.ChallengeeAthlete.RefreshChallenges();

				if(challenge.ChallengerAthlete != null)
					challenge.ChallengerAthlete.RefreshChallenges();
			});
		}

		public Task DeleteChallenge(string id)
		{
			return new Task(() =>
			{
				Challenge m;
				try
				{
					Client.GetTable<Challenge>().DeleteAsync(new Challenge {
						Id = id
					}).Wait();

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
			});
		}

		public Task GetAllChallengesByAthlete(Athlete athlete)
		{
			return new Task(() =>
			{
				var qs = new Dictionary<string, string>();
				qs.Add("athleteId", athlete.Id);
				var challenges = Client.InvokeApiAsync<string, List<Challenge>>("getChallengesForAthlete", null, HttpMethod.Get, qs).Result;
				if(challenges != null)
				{
					Challenge ch;
					var toRemove = athlete.AllChallenges.ToList();
					toRemove.ForEach(c => DataManager.Instance.Challenges.TryRemove(c.Id, out ch));
					challenges.ForEach(DataManager.Instance.Challenges.AddOrUpdate);
					athlete.RefreshChallenges();
				}
			});
		}

		public Task PostMatchResults(Challenge challenge)
		{
			return new Task(() =>
			{
				var completedChallenge = Client.InvokeApiAsync<List<GameResult>, Challenge>("postMatchResults", challenge.MatchResult).Result;
				if(completedChallenge != null)
				{
					challenge.DateCompleted = completedChallenge.DateCompleted;
					challenge.MatchResult = new List<GameResult>();
					completedChallenge.MatchResult.ForEach(challenge.MatchResult.Add);
				}
			});
		}

		public Task AcceptChallenge(Challenge challenge)
		{
			return new Task(() =>
			{
				var qs = new Dictionary<string, string>();
				qs.Add("challengeId", challenge.Id);
				var token = Client.InvokeApiAsync("acceptChallenge", null, HttpMethod.Post, qs).Result;
				var acceptedChallenge = JsonConvert.DeserializeObject<Challenge>(token.ToString());
				if(acceptedChallenge != null)
				{
					challenge.DateAccepted = acceptedChallenge.DateAccepted;
				}
			});
		}

		public Task DeclineChallenge(string challengeId)
		{
			return DeleteChallenge(challengeId);
		}

		#endregion

		#region Authentication

		public Task<UserProfile> GetUserProfile()
		{
			return new Task<UserProfile>(() =>
			{
				var client = new HttpClient();
				const string url = "https://www.googleapis.com/oauth2/v1/userinfo?alt=json";
				client.DefaultRequestHeaders.Add("Authorization", Settings.Instance.AuthToken);
				var json = client.GetStringAsync(url).Result;
				Console.WriteLine(json);
				var profile = JsonConvert.DeserializeObject<UserProfile>(json);
				return profile;
			});
		}

		public Task<string> GetNewAuthToken(string refreshToken)
		{
			return new Task<string>(() =>
			{
				const string url = "https://www.googleapis.com/oauth2/v3/token";

				using(var client = new HttpClient())
				{
					var dict = new Dictionary<string, string>();
					dict.Add("grant_type", "refresh_token");
					dict.Add("refresh_token", refreshToken);
					dict.Add("client_id", Constants.GoogleApiClientId);
					dict.Add("client_secret", Constants.GoogleClientSecret);

					var content = new FormUrlEncodedContent(dict);

					client.DefaultRequestHeaders.Add("Authorization", Settings.Instance.AuthToken);
					client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
					var response = client.PostAsync(url, content).Result;
					var body = response.Content.ReadAsStringAsync().Result;
					Console.WriteLine(body);
					dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(body);
					var newAuthToken = "{0} {1}".Fmt(dict["token_type"], dict["access_token"]);
					return newAuthToken;
				}
			});
		}

		#endregion
	}
}