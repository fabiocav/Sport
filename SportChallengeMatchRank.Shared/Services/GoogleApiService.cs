using System;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace SportChallengeMatchRank.Shared
{
	public class GoogleApiService
	{
		#region Properties

		static GoogleApiService _instance;

		public static GoogleApiService Instance
		{
			get
			{
				return _instance ?? (_instance = new GoogleApiService());
			}
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

