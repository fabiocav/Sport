using System;
using System.Threading.Tasks;
using SportChallengeMatchRank.Shared;
using Xamarin.Forms;
using nsoftware.InGoogle;
using Toasts.Forms.Plugin.Abstractions;

[assembly: Dependency(typeof(AuthenticationViewModel))]

namespace SportChallengeMatchRank.Shared
{
	public class AuthenticationViewModel : BaseViewModel
	{
		Oauth _authClient;
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
			return App.AuthUserProfile != null && App.AuthUserProfile.Email != null;
		}

		async public Task AuthenticateUser()
		{
			_authClient = new Oauth();
			_authClient.RuntimeLicense = "42474E325841314E443131474630454D30300000000000000000000000000000000000000000000030303030303030300000324D4B42325A4E59444158360000";
			_authClient.OnSSLServerAuthentication += OnSSLServerAuthentication;
			_authClient.OnLaunchBrowser += OnLaunchBrowser;

			try
			{
				AuthenticationStatus = "Checking with Google Auth";
				_authClient.ClientProfile = OauthClientProfiles.cfMobile;
				_authClient.ClientId = Constants.GoogleApiClientId;
				_authClient.ClientSecret = Constants.GoogleClientSecret;
				_authClient.ServerAuthURL = "https://accounts.google.com/o/oauth2/auth";
				_authClient.ServerTokenURL = "https://accounts.google.com/o/oauth2/token";
				_authClient.AuthorizationScope = "https://www.googleapis.com/auth/userinfo.profile https://www.googleapis.com/auth/userinfo.email https://www.googleapis.com/auth/calendar";
				var token = await _authClient.GetAuthorizationAsync();
				OnHideAuthForm();

				Settings.Instance.RefreshToken = _authClient.RefreshToken;
				Settings.Instance.AuthToken = token;

				_authClient.OnLaunchBrowser -= OnLaunchBrowser;
				_authClient.OnSSLServerAuthentication -= OnSSLServerAuthentication;

				await Settings.Instance.Save();
			}
			catch(Exception e)
			{
				//TODO Insights
				Console.WriteLine(e.GetBaseException());
			}
		}

		void OnLaunchBrowser(object sender, OauthLaunchBrowserEventArgs e)
		{
			OnDisplayAuthForm(e.URL);
		}

		void OnSSLServerAuthentication(object sender, OauthSSLServerAuthenticationEventArgs e)
		{
			e.Accept = true;
		}

		async public Task<bool> EnsureAthleteRegistered(bool forceRefresh = false)
		{
			if(App.CurrentAthlete != null && !forceRefresh)
				return true;

			Settings.Instance.AthleteId = null;
			Athlete athlete = null;
			using(new Busy(this))
			{
				AuthenticationStatus = "Getting Athlete's Profile";

				//No AthleteId on record
				if(!string.IsNullOrWhiteSpace(Settings.Instance.AthleteId))
				{
					var task = InternetService.Instance.GetAthleteById(Settings.Instance.AthleteId);
					await RunSafe(task);

					if(task.IsFaulted)
						return false;

					if(task.IsCompleted)
						athlete = task.Result;
				}

				if(athlete == null && !string.IsNullOrWhiteSpace(Settings.Instance.AuthUserID))
				{
					var task = InternetService.Instance.GetAthleteByAuthUserId(Settings.Instance.AuthUserID);
					await RunSafe(task);

					if(task.IsFaulted)
						return false;
					
					if(task.IsCompleted)
						athlete = task.Result;
				}

				if(athlete == null && App.AuthUserProfile != null && !App.AuthUserProfile.Email.IsEmpty())
				{
					var task = InternetService.Instance.GetAthleteByEmail(App.AuthUserProfile.Email);
					await RunSafe(task);

					if(task.IsFaulted)
						return false;

					if(task.IsCompleted)
						athlete = task.Result;
				}

				//Unable to get athlete - try registering as a new athlete
				if(athlete == null)
				{
					AuthenticationStatus = "Registering Athlete";
					athlete = new Athlete(App.AuthUserProfile);
					var task = InternetService.Instance.SaveAthlete(athlete);
					await RunSafe(task);

					if(task.IsCompleted && task.IsFaulted)
						return false;

					"You're now officially an athlete :)".ToToast(ToastNotificationType.Info, "Congrats!");
				}
				else
				{
					athlete.ProfileImageUrl = App.AuthUserProfile.Picture;

					if(athlete.IsDirty)
					{
						var task = InternetService.Instance.SaveAthlete(athlete);
						await RunSafe(task);
					}
				}

				Settings.Instance.AthleteId = athlete != null ? athlete.Id : null;
				await Settings.Instance.Save();

				if(App.CurrentAthlete != null)
				{
					AuthenticationStatus = "Getting joined leagues";
					await RunSafe(InternetService.Instance.GetAllLeaguesByAthlete(App.CurrentAthlete));
					AuthenticationStatus = "Getting all challenges";
					await RunSafe(InternetService.Instance.GetAllChallengesByAthlete(App.CurrentAthlete));
					await RunSafe(InternetService.Instance.UpdateAthleteNotificationHubRegistration(App.CurrentAthlete));
					MessagingCenter.Send<AuthenticationViewModel>(this, "UserAuthenticated");
				}

				AuthenticationStatus = "Done";
				return App.CurrentAthlete != null;
			}
		}

		async public Task GetUserProfile(bool force = false)
		{
			if(!force && App.AuthUserProfile != null)
				return;

			if(Settings.Instance.AuthToken == null)
			{
				AuthenticationStatus = "Authenticating with Google";
				await AuthenticateUser();
			}

			AuthenticationStatus = "Getting user profile";
			var task = InternetService.Instance.GetUserProfile();
			await RunSafe(task, false);

			if(task.IsFaulted && task.IsCompleted)
			{
				//Likely our authtoken has expired
				AuthenticationStatus = "Refreshing token";

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
				App.AuthUserProfile = task.Result;
				Settings.Instance.AuthUserID = App.AuthUserProfile.Id;
				await Settings.Instance.Save();
			}
			else
			{
				AuthenticationStatus = "Unable to authenticate";
			}
		}

		public void LogOut()
		{
			Settings.Instance.AthleteId = null;
			Settings.Instance.AuthUserID = null;
			Settings.Instance.AuthToken = null;
			Settings.Instance.RefreshToken = null;
			Settings.Instance.Save();
			App.AuthUserProfile = null;
		}

		public override void Dispose()
		{
			base.Dispose();

//			_authClient.OnSSLServerAuthentication -= OnSSLServerAuthentication;
//			_authClient.OnLaunchBrowser -= OnLaunchBrowser;
		}
	}
}