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
			App.AuthUserProfile.Email != null;
//			App.AuthUserProfile.Email.EndsWith("@xamarin.com", StringComparison.OrdinalIgnoreCase) &&
			//App.AuthUserProfile.EmailVerified;
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
			if(App.CurrentAthlete != null && !forceRefresh)
				return true;

			Settings.Instance.AthleteId = null;
			Athlete athlete = null;

			//No AthleteId on record
			if(!string.IsNullOrWhiteSpace(Settings.Instance.AthleteId))
			{
				var task = AzureService.Instance.GetAthleteById(Settings.Instance.AthleteId);
				await RunSafe(task);

				if(task.IsCompleted)
					athlete = task.Result;
			}

			if(athlete == null && !string.IsNullOrWhiteSpace(Settings.Instance.AuthUserID))
			{
				var task = AzureService.Instance.GetAthleteByAuthUserId(Settings.Instance.AuthUserID);
				await RunSafe(task);

				if(task.IsCompleted)
					athlete = task.Result;
			}

			if(athlete == null && App.AuthUserProfile != null && !string.IsNullOrWhiteSpace(App.AuthUserProfile.Email))
			{
				var task = AzureService.Instance.GetAthleteByEmail(App.AuthUserProfile.Email);
				await RunSafe(task);

				if(task.IsCompleted)
					athlete = task.Result;
			}

			//Unable to get athlete - add as new
			if(athlete == null)
			{
				athlete = new Athlete(App.AuthUserProfile);
				await RunSafe(AzureService.Instance.SaveAthlete(athlete));
			}

			Settings.Instance.AthleteId = athlete != null ? athlete.Id : null;
			await Settings.Instance.Save();

			if(App.CurrentAthlete != null)
			{
				await RunSafe(AzureService.Instance.GetAllLeaguesByAthlete(App.CurrentAthlete));
				await RunSafe(AzureService.Instance.GetAllChallengesByAthlete(App.CurrentAthlete));
				await RunSafe(AzureService.Instance.UpdateAthleteRegistrationForPush());
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
					string json;
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
						LogOut();
					}
				}
				catch(Exception e)
				{
					OnTaskException(e);
					Console.WriteLine("Error getting user profile: {0}", e);
				}
			}
		}
	}
}