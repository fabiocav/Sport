using System;
using Xamarin.Forms;
using System.Threading.Tasks;
using SportRankerMatchOn.Shared;

namespace SportRankerMatchOn.Shared
{
	public class AuthenticationPage : BaseContentPage<AuthenticationViewModel>
	{
		Label _userLabel;
		Label _statusLabel;
		Button _loginButton;
		Button _logoutButton;
		ActivityIndicator _activity;
		bool _isFistLoad = true;

		public AuthenticationPage()
		{
			Title = "Authentication";
			InitializeInterface();
		}

		async public Task UserAuthenticationUpdated()
		{
			if(!ViewModel.IsUserValid())
			{
				App.AuthUserProfile = null;
				await DisplayAlert("Invalid Email", "This service is only available to Xamarin employees.", "OK");
			}
			else
			{
				if(App.AuthUserProfile != null)
				{
					await ViewModel.EnsureAthleteRegistered();
				}

				await Navigation.PushAsync(new AdminPage());
			}

			_userLabel.Text = App.AuthUserProfile == null ? "empty" : App.AuthUserProfile.Email;
			_loginButton.IsVisible = App.AuthUserProfile == null;
			_logoutButton.IsVisible = !_loginButton.IsVisible;
		}

		async protected override void OnAppearing()
		{
			if(_isFistLoad)
			{
				_isFistLoad = false;
				await AttemptToReauthenticateAthlete();
			}

			base.OnAppearing();
		}

		async public Task AttemptToReauthenticateAthlete()
		{
			_statusLabel.Text = "Getting user profile...";
			await ViewModel.GetUserProfile();
			if(App.AuthUserProfile != null)
			{
				_statusLabel.Text = "Checking athlete registration...";
				await ViewModel.EnsureAthleteRegistered();
			}
			else
			{
				MessagingCenter.Send<AuthenticationPage>(this, "AuthenticateUser");
			}

			await UserAuthenticationUpdated();
		}

		void InitializeInterface()
		{
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

			_loginButton = new Button {
				BackgroundColor = Color.Gray,
				TextColor = Color.Black,
				IsVisible = false,
				Text = "Log In",
			};

			_logoutButton = new Button {
				BackgroundColor = Color.Gray,
				TextColor = Color.Black,
				IsVisible = false,
				Text = "Log Out",
			};

			_loginButton.Clicked += (sender, e) =>
			{
				MessagingCenter.Send<AuthenticationPage>(this, "AuthenticateUser");
			};

			_logoutButton.Clicked += (sender, e) =>
			{
				ViewModel.LogOut();
				MessagingCenter.Send<AuthenticationPage>(this, "AuthenticateUser");
			};

			Content = new StackLayout {
				Padding = 40,
				Spacing = 20,
				VerticalOptions = LayoutOptions.Center,
				Children = {
						_activity,
						_statusLabel,
						_userLabel,
					_loginButton,
					_logoutButton
				}
			};
		}
	}
}