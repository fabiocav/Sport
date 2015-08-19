using System;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace Sport.Shared
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

		public Task<bool> ValidateToken(string token)
		{
			return new Task<bool>(() =>
			{
				var client = new HttpClient();
				string url = "https://www.googleapis.com/oauth2/v1/tokeninfo?access_token=" + token.TrimStart("Bearer ");
				var json = client.GetStringAsync(url).Result;
				var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
				return result.ContainsKey("user_id");
			});
		}


		public Task<UserProfile> GetUserProfile()
		{
			return new Task<UserProfile>(() =>
			{
				var client = new HttpClient();
				const string url = "https://www.googleapis.com/oauth2/v1/userinfo?alt=json";
				client.DefaultRequestHeaders.Add("Authorization", App.AuthToken);
				var json = client.GetStringAsync(url).Result;
				var profile = JsonConvert.DeserializeObject<UserProfile>(json);
				return profile;
			});
		}

		public Task<Tuple<string, string>> GetAuthAndRefreshToken(string code)
		{
			return new Task<Tuple<string, string>>(() =>
			{
				string url = "https://www.googleapis.com/oauth2/v3/token";

				using(var client = new HttpClient())
				{
					var dict = new Dictionary<string, string>();
					dict.Add("code", code);
					dict.Add("grant_type", "authorization_code");
					dict.Add("redirect_uri", "urn:ietf:wg:oauth:2.0:oob");
					dict.Add("client_id", Keys.GoogleApiClientId);
					dict.Add("client_secret", Keys.GoogleClientSecret);

					var content = new FormUrlEncodedContent(dict);
					client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
					var response = client.PostAsync(url, content).Result;
					var body = response.Content.ReadAsStringAsync().Result;
					dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(body);

					if(!dict.ContainsKey("token_type"))
					{
						return null;
					}

					var newAuthToken = "{0} {1}".Fmt(dict["token_type"], dict["access_token"]);
					var refreshToken = dict["refresh_token"];
					return new Tuple<string, string>(newAuthToken, refreshToken);
				}
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
					dict.Add("client_id", Keys.GoogleApiClientId);
					dict.Add("client_secret", Keys.GoogleClientSecret);

					var content = new FormUrlEncodedContent(dict);

					client.DefaultRequestHeaders.Add("Authorization", App.AuthToken);
					client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
					var response = client.PostAsync(url, content).Result;
					var body = response.Content.ReadAsStringAsync().Result;
					dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(body);

					if(!dict.ContainsKey("token_type"))
					{
						return null;
					}

					var newAuthToken = "{0} {1}".Fmt(dict["token_type"], dict["access_token"]);
					return newAuthToken;
				}
			});
		}

		#endregion
	}
}

