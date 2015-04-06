
using System;
using Xamarin.Forms;
using System.Threading.Tasks;
using SportChallengeMatchRank.Shared;
using Toasts.Forms.Plugin.Abstractions;

namespace SportChallengeMatchRank.Shared
{
	public class AuthenticationPage : BaseContentPage<AuthenticationViewModel>
	{
		Label _userLabel;
		Label _statusLabel;
		Button _loginButton;
		Button _logoutButton;
		Button _adminButton;
		Button _athleteLandingButton;
		ActivityIndicator _activity;

		public AuthenticationPage()
		{
			Initialize();
		}

		async public Task UserAuthenticationUpdated()
		{
			if(!ViewModel.IsUserValid())
			{
				if(App.AuthUserProfile != null)
				{
					App.AuthUserProfile = null;
					"This service is only available to the xamarin.com organization.".ToToast(ToastNotificationType.Warning, "Invalid Organization");
				}
			}
			else
			{
				if(App.CurrentAthlete != null)
				{
					await ViewModel.RunSafe(() => ViewModel.EnsureAthleteRegistered());
				}

				await Navigation.PopModalAsync();
			}

			_userLabel.Text = App.AuthUserProfile == null ? "empty" : App.AuthUserProfile.Email;
			_loginButton.IsVisible = App.AuthUserProfile == null;
			_logoutButton.IsVisible = !_loginButton.IsVisible;
		}

		async protected override void OnLoaded()
		{
			MessagingCenter.Subscribe<AzureService, string>(this, "ServiceCallFailed", (sender, message) =>
				{
					Console.WriteLine("Notification received from ServiceCallFailed");
					message.ToToast(ToastNotificationType.Error);
				});
			
			await AttemptToReauthenticateAthlete();
			base.OnLoaded();
		}

		protected override void OnAppearing()
		{
			_adminButton.IsVisible = App.CurrentAthlete != null && App.CurrentAthlete.IsAdmin;
			_athleteLandingButton.IsVisible = App.CurrentAthlete != null;
			base.OnAppearing();
		}

		async public Task AttemptToReauthenticateAthlete()
		{
			_statusLabel.Text = "Getting user profile...";
			await ViewModel.GetUserProfile();
			if(App.AuthUserProfile != null)
			{
				_statusLabel.Text = "Checking athlete registration...";

				try
				{
					var success = await ViewModel.RunSafe(() => ViewModel.EnsureAthleteRegistered());
					Console.WriteLine(success);
				}
				catch(Exception e)
				{
					Console.WriteLine(e);
				}
			}
			else
			{
				MessagingCenter.Send<AuthenticationViewModel>(ViewModel, "AuthenticateUser");
			}

			_statusLabel.Text = string.Empty;
			await UserAuthenticationUpdated();
		}

		protected override void Initialize()
		{
			Title = "Authentication";
			BackgroundColor = Color.White;

			_activity = new ActivityIndicator();
			_activity.SetBinding(ActivityIndicator.IsRunningProperty, "IsBusy");
			_activity.SetBinding(VisualElement.IsVisibleProperty, "IsBusy");
			_activity.Color = Color.Gray;

			_statusLabel = new Label();
			_statusLabel.TextColor = Color.Black;
			_statusLabel.FontSize = 18;
			_statusLabel.HorizontalOptions = LayoutOptions.Center;

			_userLabel = new Label();
			_userLabel.TextColor = Color.Black;
			_userLabel.FontSize = 18;
			_userLabel.HorizontalOptions = LayoutOptions.Center;

			_adminButton = new Button {
				BackgroundColor = Color.Gray,
				TextColor = Color.White,
				Text = "Go to Admin",
			};

			_athleteLandingButton = new Button {
				BackgroundColor = Color.Gray,
				TextColor = Color.White,
				Text = "Go to Athlete Landing",
				IsVisible = false,
			};

			_loginButton = new Button {
				BackgroundColor = Color.Gray,
				TextColor = Color.White,
				IsVisible = false,
				Text = "Log In",
			};

			_logoutButton = new Button {
				BackgroundColor = Color.Gray,
				TextColor = Color.White,
				IsVisible = false,
				Text = "Log Out",
			};

			_loginButton.Clicked += (sender, e) =>
			{
				MessagingCenter.Send<AuthenticationViewModel>(ViewModel, "AuthenticateUser");
			};

			_logoutButton.Clicked += (sender, e) =>
			{
				ViewModel.LogOut();
				MessagingCenter.Send<AuthenticationViewModel>(ViewModel, "AuthenticateUser");
			};

			_adminButton.Clicked += (sender, e) =>
			{
				Navigation.PushAsync(new AdminPage());		
			};

			_athleteLandingButton.Clicked += (sender, e) =>
			{
				Navigation.PushAsync(new AthleteLeaguesPage());		
			};
			
			Content = new StackLayout {
				Padding = 40,
				Spacing = 20,
				VerticalOptions = LayoutOptions.Center,
				Children = {
					_activity,
					_statusLabel,
					_userLabel,
					_athleteLandingButton,
						_adminButton,
						_loginButton,
						_logoutButton
				}
			};
		}
	}
}