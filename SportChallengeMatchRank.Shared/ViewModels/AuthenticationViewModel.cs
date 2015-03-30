using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SportChallengeMatchRank.Shared;
using Xamarin.Forms;

[assembly: Dependency(typeof(AuthenticationViewModel))]

namespace SportChallengeMatchRank.Shared
{
	public class AuthenticationViewModel : BaseViewModel
	{
		public bool IsUserValid()
		{
			return App.AuthUserProfile != null &&
			App.AuthUserProfile.Email != null &&
//			App.AuthUserProfile.Email.EndsWith("@xamarin.com", StringComparison.OrdinalIgnoreCase) &&
			App.AuthUserProfile.EmailVerified;
		}

		public void LogOut()
		{
			Settings.Instance.AuthToken = null;
			Settings.Instance.AuthUserID = null;
			App.AuthUserProfile = null;
			Settings.Instance.Save();
		}

		async public Task<bool> EnsureAthleteRegistered(bool forceRefresh = false)
		{
			using(new Busy(this))
			{
				if(App.CurrentAthlete != null && !forceRefresh)
					return true;

				Settings.Instance.AthleteId = null;
				Athlete athlete = null;

				//No AthleteId on record
				if(!string.IsNullOrWhiteSpace(Settings.Instance.AthleteId))
				{
					athlete = await AzureService.Instance.GetAthleteById(Settings.Instance.AthleteId);
				}

				if(athlete == null && !string.IsNullOrWhiteSpace(Settings.Instance.AuthUserID))
				{
					athlete = await AzureService.Instance.GetAthleteByAuthUserId(Settings.Instance.AuthUserID);
				}

				if(athlete == null && App.AuthUserProfile != null && !string.IsNullOrWhiteSpace(App.AuthUserProfile.Email))
				{
					athlete = await AzureService.Instance.GetAthleteByEmail(App.AuthUserProfile.Email);
				}

				//Unable to get athlete - add as new
				if(athlete == null)
				{
					athlete = new Athlete(App.AuthUserProfile);
					await AzureService.Instance.SaveAthlete(athlete);
				}

				Settings.Instance.AthleteId = athlete != null ? athlete.Id : null;
				await Settings.Instance.Save();
			}

			if(App.CurrentAthlete != null)
			{
				await AzureService.Instance.GetAllLeaguesByAthlete(App.CurrentAthlete);
				await AzureService.Instance.UpdateAthleteRegistrationForPush();
			}

			return App.CurrentAthlete != null;
		}

		async public Task GetUserProfile(bool force = false)
		{
			if(!force && (App.AuthUserProfile != null || string.IsNullOrWhiteSpace(Settings.Instance.AuthToken) || string.IsNullOrWhiteSpace(Settings.Instance.AuthUserID)))
				return;

			using(new Busy(this))
			{
				try
				{
					string json = null;
					using(var client = new HttpClient())
					{
						client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer {0}".Fmt(Settings.Instance.AuthToken));
						var url = "https://{0}/api/users/{1}".Fmt(Constants.AuthDomain, Settings.Instance.AuthUserID);
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
						//Attempt to renew token

						//LogOut();
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