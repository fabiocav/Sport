using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleAuth;
using SimpleAuth.Providers;
using Sport.Shared;
using Xamarin;
using Xamarin.Forms;

[assembly: Dependency(typeof(AuthenticationViewModel))]

namespace Sport.Shared
{
	public class AuthenticationViewModel : BaseViewModel
	{
		bool _doResetWebCache;

		#region Properties

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

		internal UserProfile AuthUserProfile
		{
			get;
			set;
		}

		#endregion

		/// <summary>
		/// Performs a complete authentication pass
		/// </summary>
		public async Task<bool> AuthenticateCompletely()
		{
			await AuthenticateWithGoogle();

			if(AuthUserProfile != null)
				await AuthenticateWithBackend();

			return App.CurrentAthlete != null;
		}

		/// <summary>
		/// Attempts to get the user's profile and will use WebView form to authenticate if necessary
		/// </summary>
		async Task<bool> AuthenticateWithGoogle()
		{
			var authViewModel = DependencyService.Get<AuthenticationViewModel>();
			var success = await authViewModel.GetUserProfile();

			if(!success)
			{
				await ShowGoogleAuthenticationView();
				await authViewModel.GetUserProfile();
			}

			return AuthUserProfile != null;
		}

		/// <summary>
		/// Shows the Google authentication web view so the user can authenticate
		/// </summary>
		async Task ShowGoogleAuthenticationView()
		{
			var scopes = new[] {
				"email",
				"profile",
				"https://www.googleapis.com/auth/calendar"
			};

			var api = new GoogleApi("google", Keys.GoogleApiClientId, Keys.GoogleClientSecret) {
				Scopes = scopes,
			};

			if(_doResetWebCache)
			{
				_doResetWebCache = false;				
				api.ResetData();
			}

			var account = await api.Authenticate();

			if(account != null)
			{
				var oauthAccount = (OAuthAccount)account;

				Settings.Instance.AuthToken = oauthAccount.Token;
				Settings.Instance.RefreshToken = oauthAccount.RefreshToken;
				Settings.Instance.AuthUserID = oauthAccount.Identifier;
				await Settings.Instance.Save();
			}
		}

		/// <summary>
		/// Authenticates the athlete against the Azure backend and loads all necessary data to begin the app experience
		/// </summary>
		async Task<bool> AuthenticateWithBackend()
		{
			Athlete athlete;
			using(new Busy(this))
			{
				AuthenticationStatus = "Getting athlete's profile";
				athlete = await GetAthletesProfile();

				if(athlete == null)
				{
					//Unable to get athlete - try registering as a new athlete
					athlete = await RegisterAthlete(AuthUserProfile);
				}
				else
				{
					athlete.ProfileImageUrl = AuthUserProfile.Picture;

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
					await GetAllLeaderboards();
					App.CurrentAthlete.IsDirty = false;
					MessagingCenter.Send<AuthenticationViewModel>(this, "UserAuthenticated");
				}

				AuthenticationStatus = "Done";
				return App.CurrentAthlete != null;
			}
		}

		/// <summary>
		/// Gets the athlete's profile from the Azure backend
		/// </summary>
		async Task<Athlete> GetAthletesProfile()
		{
			Athlete athlete = null;

			//Let's try to load based on email address
			if(athlete == null && AuthUserProfile != null && !AuthUserProfile.Email.IsEmpty())
			{
				var task = AzureService.Instance.GetAthleteByEmail(AuthUserProfile.Email);
				await RunSafe(task);

				if(task.IsCompleted && !task.IsFaulted)
					athlete = task.Result;
			}

			return athlete;
		}


		/// <summary>
		/// Registers an athlete with the backend and returns the new athlete profile
		/// </summary>
		async Task<Athlete> RegisterAthlete(UserProfile profile)
		{
			AuthenticationStatus = "Registering athlete";
			var athlete = new Athlete(profile);

			var task = AzureService.Instance.SaveAthlete(athlete);
			await RunSafe(task);

			if(task.IsCompleted && task.IsFaulted)
				return null;

			"You're now an officially registered athlete!".ToToast();
			return athlete;
		}

		/// <summary>
		/// Gets all leaderboards for the system
		/// </summary>
		async Task GetAllLeaderboards()
		{
			AuthenticationStatus = "Getting leaderboards";
			var task = AzureService.Instance.GetAllLeaguesForAthlete(App.CurrentAthlete);
			await RunSafe(task);

			if(task.IsCompleted && !task.IsFaulted)
			{
				App.Current.UsedLeagueColors.Clear();
				task.Result.EnsureLeaguesThemed();
			}
		}

		/// <summary>
		/// Attempts to get the user profile from Google. Will use the refresh token if the auth token has expired
		/// </summary>
		async public Task<bool> GetUserProfile()
		{
			//Can't get profile w/out a token
			if(Settings.Instance.AuthToken == null)
				return false;

			using(new Busy(this))
			{
				AuthenticationStatus = "Getting Google user profile";
				var task = GoogleApiService.Instance.GetUserProfile();
				await RunSafe(task, false);

				if(task.IsFaulted && task.IsCompleted)
				{
					//Likely our authtoken has expired
					AuthenticationStatus = "Refreshing token";

					var refreshTask = GoogleApiService.Instance.GetNewAuthToken(Settings.Instance.RefreshToken);
					await RunSafe(refreshTask);

					if(refreshTask.IsCompleted && !refreshTask.IsFaulted)
					{
						//Success in getting a new auth token - now lets attempt to get the profile again
						if(!string.IsNullOrWhiteSpace(refreshTask.Result) && Settings.Instance.AuthToken != refreshTask.Result)
						{
							//We have a valid token now, let's try this again
							Settings.Instance.AuthToken = refreshTask.Result;
							await Settings.Instance.Save();
							return await GetUserProfile();
						}
					}
				}

				if(task.IsCompleted && !task.IsFaulted && task.Result != null)
				{
					AuthenticationStatus = "Authentication complete";
					AuthUserProfile = task.Result;

					Insights.Identify(AuthUserProfile.Email, new Dictionary<string, string> { {
							"Name",
							AuthUserProfile.Name
						}
					});

					Settings.Instance.AuthUserID = AuthUserProfile.Id;
					await Settings.Instance.Save();
				}
				else
				{
					AuthenticationStatus = "Unable to authenticate";
					_doResetWebCache = true;
				}
			}

			return AuthUserProfile != null;
		}

		public void LogOut()
		{
			_doResetWebCache = true;

			Settings.Instance.AthleteId = null;
			Settings.Instance.AuthUserID = null;
			Settings.Instance.AuthToken = null;
			Settings.Instance.RefreshToken = null;
			Settings.Instance.RegistrationComplete = false;
			Settings.Instance.Save();
			AuthUserProfile = null;
		}
	}
}