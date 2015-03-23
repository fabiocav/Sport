using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SportRankerMatchOn.Shared;
using Xamarin.Forms;

[assembly: Dependency(typeof(SportRankerMatchOn.Shared.AuthenticationViewModel))]

namespace SportRankerMatchOn.Shared
{
	public class AuthenticationViewModel : BaseViewModel
	{
		public bool IsUserValid()
		{
			return App.AuthUserProfile != null &&
			App.AuthUserProfile.Email != null &&
			App.AuthUserProfile.Email.EndsWith("@xamarin.com", StringComparison.OrdinalIgnoreCase) &&
			App.AuthUserProfile.EmailVerified;
		}

		public void LogOut()
		{
			AppSettings.AuthToken = null;
			AppSettings.AuthUserID = null;
			App.AuthUserProfile = null;
		}

		async public Task<bool> EnsureAthleteRegistered()
		{
			using(new Busy(this))
			{
				if(string.IsNullOrWhiteSpace(AppSettings.AthleteId))
				{
					var athlete = new Athlete(App.AuthUserProfile);
					await AzureService.Instance.SaveAthlete(athlete);
					AppSettings.AthleteId = athlete.Id;
					App.CurrentAthlete = athlete;
				}
				else if(App.CurrentAthlete == null)
				{
					App.CurrentAthlete = await AzureService.Instance.GetAthleteById(AppSettings.AthleteId);
					if(App.CurrentAthlete == null)
					{
						var athlete = new Athlete(App.AuthUserProfile);
						await AzureService.Instance.SaveAthlete(athlete);
						AppSettings.AthleteId = athlete.Id;
						App.CurrentAthlete = athlete;
					}
				}
			}

			return App.CurrentAthlete != null;
		}

		async public Task GetUserProfile(bool force = false)
		{
			if(!force && (App.AuthUserProfile != null || string.IsNullOrWhiteSpace(AppSettings.AuthToken) || string.IsNullOrWhiteSpace(AppSettings.AuthUserID)))
				return;

			using(new Busy(this))
			{
				try
				{
					string json = null;
					using(var client = new HttpClient())
					{
						client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer {0}".Fmt(AppSettings.AuthToken));
						var url = "https://{0}/api/users/{1}".Fmt(Constants.AuthDomain, AppSettings.AuthUserID);
						json = await client.GetStringAsync(url);
					}

					if(json != null)
					{
						App.AuthUserProfile = JsonConvert.DeserializeObject<UserProfile>(json);
					}
				}
				catch(HttpRequestException hre)
				{
					if(hre.Message.ContainsNoCase("unauthorized"))
					{
						LogOut();
					}
				}
				catch(Exception e)
				{
					Console.WriteLine("Error getting user profile: {0}", e);
				}
			}
		}
	}
}