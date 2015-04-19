using System;
using System.Threading.Tasks;
using SportChallengeMatchRank.Shared;
using Xamarin.Forms;
using nsoftware.InGoogle;

[assembly: Dependency(typeof(AuthenticationViewModel))]

namespace SportChallengeMatchRank.Shared
{
	public class AuthenticationViewModel : BaseViewModel
	{
		string _authenticationStatus;

		public string AuthenticationStatus
		{
			get
			{
				return _authenticationStatus;
			}
			set
			{
				SetPropertyChanged(ref _authenticationStatus, value);
			}
		}

		public Action<string> OnDisplayAuthForm
		{
			get;
			set;
		}

		public Action OnHideAuthForm
		{
			get;
			set;
		}

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

		async public Task AuthenticateUser()
		{
			var auth = new Oauth();

			auth.OnSSLServerAuthentication += (s, e) =>
			{
				e.Accept = true;
				//auth.OnSSLServerAuthentication.GetInvocationList().ToList().ForEach(p => auth.OnSSLServerAuthentication -= p);
			};

			auth.OnLaunchBrowser += (sender, e) =>
			{
				OnDisplayAuthForm(e.URL);
				//auth.OnLaunchBrowser.GetInvocationList().ToList().ForEach(p => auth.OnLaunchBrowser -= p);
			};

			try
			{
				AuthenticationStatus = "Checking with Google Auth";
				await Task.Delay(1000);
				auth.ClientProfile = OauthClientProfiles.cfMobile;
				auth.ClientId = Constants.GoogleApiClientId;
				auth.ClientSecret = Constants.GoogleClientSecret;
				auth.ServerAuthURL = "https://accounts.google.com/o/oauth2/auth";
				auth.ServerTokenURL = "https://accounts.google.com/o/oauth2/token";
				auth.AuthorizationScope = "https://www.googleapis.com/auth/userinfo.profile https://www.googleapis.com/auth/userinfo.email https://www.googleapis.com/auth/calendar";
				var token = await auth.GetAuthorizationAsync();
				OnHideAuthForm();

				Settings.Instance.RefreshToken = auth.RefreshToken;
				Settings.Instance.AuthToken = token;
				await Settings.Instance.Save();
			}
			catch(Exception e)
			{
				Console.WriteLine(e.GetBaseException());
			}
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
				var task = InternetService.Instance.GetAthleteById(Settings.Instance.AthleteId);
				await RunSafe(task);

				if(task.IsCompleted)
					athlete = task.Result;
			}

			if(athlete == null && !string.IsNullOrWhiteSpace(Settings.Instance.AuthUserID))
			{
				var task = InternetService.Instance.GetAthleteByAuthUserId(Settings.Instance.AuthUserID);
				await RunSafe(task);

				if(task.IsCompleted)
					athlete = task.Result;
			}

			if(athlete == null && App.AuthUserProfile != null && !string.IsNullOrWhiteSpace(App.AuthUserProfile.Email))
			{
				var task = InternetService.Instance.GetAthleteByEmail(App.AuthUserProfile.Email);
				await RunSafe(task);

				if(task.IsCompleted)
					athlete = task.Result;
			}

			//Unable to get athlete - add as new
			if(athlete == null)
			{
				athlete = new Athlete(App.AuthUserProfile);
				await RunSafe(InternetService.Instance.SaveAthlete(athlete));
			}

			Settings.Instance.AthleteId = athlete != null ? athlete.Id : null;
			await Settings.Instance.Save();

			if(App.CurrentAthlete != null)
			{
				await RunSafe(InternetService.Instance.GetAllLeaguesByAthlete(App.CurrentAthlete));
				await RunSafe(InternetService.Instance.GetAllChallengesByAthlete(App.CurrentAthlete));
				await RunSafe(InternetService.Instance.UpdateAthleteRegistrationForPush());
			}

			return App.CurrentAthlete != null;
		}

		async public Task GetUserProfile(bool force = false)
		{
			if(!force && App.AuthUserProfile != null)
				return;

			if(Settings.Instance.AuthToken == null)
			{
				AuthenticationStatus = "Authenticating with Google";
				await Task.Delay(1000);
				await AuthenticateUser();
			}

			AuthenticationStatus = "Getting user profile";
			await Task.Delay(1000);
			var task = InternetService.Instance.GetUserProfile();
			await RunSafe(task);

			if(task.IsFaulted && task.IsCompleted)
			{
				//Likely our authtoken has expired
				AuthenticationStatus = "Refreshing token";
				await Task.Delay(1000);

				var refreshTask = InternetService.Instance.GetNewAuthToken(Settings.Instance.RefreshToken);
				await RunSafe(refreshTask);

				if(refreshTask.IsCompleted && !refreshTask.IsFaulted)
				{
					//Succes in getting a new auth token - now lets attempt to get the profile again
					if(!string.IsNullOrWhiteSpace(refreshTask.Result) && Settings.Instance.AuthToken != refreshTask.Result)
					{
						Settings.Instance.AuthToken = refreshTask.Result;
						await Settings.Instance.Save();
						await GetUserProfile();
					}
				}

				return;
			}

			if(task.IsCompleted && !task.IsFaulted)
			{
				AuthenticationStatus = "Authentication complete";
				await Task.Delay(1000);
				App.AuthUserProfile = task.Result;
			}
			else
			{
				AuthenticationStatus = "Unable to authenticate";
				await Task.Delay(1000);
			}
		}
	}
}