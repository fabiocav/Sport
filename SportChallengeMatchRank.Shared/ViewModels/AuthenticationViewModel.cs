using System;
using System.Threading.Tasks;
using SportChallengeMatchRank.Shared;
using Xamarin.Forms;
using nsoftware.InGoogle;
using Xamarin;
using System.Collections.Generic;
using System.Net;

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

		public Action<string, Oauth> OnDisplayAuthForm
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

		async public Task<bool> AuthenticateUser()
		{
			if(!App.IsNetworkRechable)
			{
				NotifyException(new WebException("Please connect to the Information Super Highway"));
				return false;
			}

			_authClient = new Oauth();
			_authClient.RuntimeLicense = "42474E325841314E443131474630454D30300000000000000000000000000000000000000000000030303030303030300000324D4B42325A4E59444158360000";
			_authClient.OnSSLServerAuthentication += OnSSLServerAuthentication;
			_authClient.OnLaunchBrowser += OnLaunchBrowser;
			//_authClient.ReturnURL = "https://dl.dropboxusercontent.com/u/54307520/sport-challenge-auth-redirect.html";

			try
			{
				AuthenticationStatus = "Checking Google auth";
				_authClient.ClientProfile = OauthClientProfiles.cfMobile;
				_authClient.ClientId = Constants.GoogleApiClientId;
				_authClient.ClientSecret = Constants.GoogleClientSecret;
				_authClient.ServerAuthURL = "https://accounts.google.com/o/oauth2/auth";
				_authClient.ServerTokenURL = "https://accounts.google.com/o/oauth2/token";
				_authClient.AuthorizationScope = "https://www.googleapis.com/auth/userinfo.profile https://www.googleapis.com/auth/userinfo.email https://www.googleapis.com/auth/calendar";
				var token = await _authClient.GetAuthorizationAsync();

				//Gets hit when user is redirected to success URL
				OnHideAuthForm();

				Settings.Instance.RefreshToken = _authClient.RefreshToken;
				Settings.Instance.AuthToken = token;

				_authClient.OnLaunchBrowser -= OnLaunchBrowser;
				_authClient.OnSSLServerAuthentication -= OnSSLServerAuthentication;

				await Settings.Instance.Save();
				return true;
			}
			catch(Exception e)
			{
				//TODO Insights
				Console.WriteLine(e.GetBaseException());
				return false;
			}
		}

		void OnLaunchBrowser(object sender, OauthLaunchBrowserEventArgs e)
		{
			OnDisplayAuthForm(e.URL, _authClient);
		}

		void OnSSLServerAuthentication(object sender, OauthSSLServerAuthenticationEventArgs e)
		{
			e.Accept = true;
		}

		async public Task<bool> EnsureAthleteRegistered(bool forceRefresh = false)
		{
			if(App.CurrentAthlete != null && !forceRefresh)
				return true;

			Athlete athlete = null;
			using(new Busy(this))
			{
				AuthenticationStatus = "Getting athlete's profile";

				//No AthleteId on record
				if(!string.IsNullOrWhiteSpace(Settings.Instance.AthleteId))
				{
					var task = AzureService.Instance.GetAthleteById(Settings.Instance.AthleteId);
					await RunSafe(task);

					if(task.IsCompleted && !task.IsFaulted)
						athlete = task.Result;
				}

				if(athlete == null && !string.IsNullOrWhiteSpace(Settings.Instance.AuthUserID))
				{
					var task = AzureService.Instance.GetAthleteByAuthUserId(Settings.Instance.AuthUserID);
					await RunSafe(task);

					if(task.IsCompleted && !task.IsFaulted)
						athlete = task.Result;
				}

				if(athlete == null && App.AuthUserProfile != null && !App.AuthUserProfile.Email.IsEmpty())
				{
					var task = AzureService.Instance.GetAthleteByEmail(App.AuthUserProfile.Email);
					await RunSafe(task);

					if(task.IsCompleted && !task.IsFaulted)
						athlete = task.Result;
				}

				//Unable to get athlete - try registering as a new athlete
				if(athlete == null)
				{
					AuthenticationStatus = "Registering athlete";
					athlete = new Athlete(App.AuthUserProfile);
					var task = AzureService.Instance.SaveAthlete(athlete);
					await RunSafe(task);

					if(task.IsCompleted && task.IsFaulted)
						return false;

					"Registered".ToToast();
				}
				else
				{
					athlete.ProfileImageUrl = App.AuthUserProfile.Picture;

					if(athlete.IsDirty)
					{
						var task = AzureService.Instance.SaveAthlete(athlete);
						await RunSafe(task);
					}
				}

				Settings.Instance.AthleteId = athlete?.Id;
				await Settings.Instance.Save();

				if(App.CurrentAthlete != null)
				{
					AuthenticationStatus = "Getting leaderboards";
					var task = AzureService.Instance.GetAllLeaguesForAthlete(App.CurrentAthlete);
					await RunSafe(task);

					App.CurrentAthlete.IsDirty = false;
					//await RunSafe(AzureService.Instance.UpdateAthleteNotificationHubRegistration(App.CurrentAthlete));
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

			using(new Busy(this))
			{
				if(Settings.Instance.AuthToken == null)
				{
					AuthenticationStatus = "Authenticating with Google";
					var success = await AuthenticateUser();

					if(!success)
						return;
				}

				AuthenticationStatus = "Getting Google user profile";
				var task = GoogleApiService.Instance.GetUserProfile();
				await RunSafe(task, false);

				if(task.IsFaulted && task.IsCompleted)
				{
					//Likely our authtoken has expired
					AuthenticationStatus = "Refreshing token";

					var refreshTask = GoogleApiService.Instance.GetNewAuthToken(Settings.Instance.RefreshToken);
					await RunSafe(refreshTask);

					var invalid = true;
					if(refreshTask.IsCompleted && !refreshTask.IsFaulted)
					{
						//Success in getting a new auth token - now lets attempt to get the profile again
						if(!string.IsNullOrWhiteSpace(refreshTask.Result) && Settings.Instance.AuthToken != refreshTask.Result)
						{
							invalid = false;
							Settings.Instance.AuthToken = refreshTask.Result;
							await Settings.Instance.Save();
							await GetUserProfile();
						}
					}

					if(invalid)
					{
						Settings.Instance.AuthToken = null;
						Settings.Instance.RefreshToken = null;
						await GetUserProfile(force);
					}
				}

				if(task.IsCompleted && !task.IsFaulted)
				{
					AuthenticationStatus = "Authentication complete";
					App.AuthUserProfile = task.Result;

					Insights.Identify(App.AuthUserProfile.Email, new Dictionary<string, string> { {
							"Name",
							App.AuthUserProfile.Name
						}
					});

					Settings.Instance.AuthUserID = App.AuthUserProfile.Id;
					await Settings.Instance.Save();
				}
				else
				{
					AuthenticationStatus = "Unable to authenticate";
				}
			}
		}

		public void LogOut()
		{
			Settings.Instance.AthleteId = null;
			Settings.Instance.AuthUserID = null;
			Settings.Instance.AuthToken = null;
			Settings.Instance.RefreshToken = null;
			Settings.Instance.RegistrationComplete = false;
			Settings.Instance.Save();
			App.AuthUserProfile = null;
		}
	}
}