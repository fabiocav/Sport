using System;
using Xamarin.Forms;
using System.Threading.Tasks;
using SportRankerMatchOn.Shared;

namespace SportRankerMatchOn.Shared
{
	public class AuthenticationPage : BaseContentPage<AuthenticationViewModel>
	{
		Label _userLabel;
		Button _loginButton;
		Button _logoutButton;
		ActivityIndicator _activity;

		public AuthenticationPage()
		{
			Initialize();
			Title = "Authentication";
			CheckForAuthentication();

		}

		async public void UserAuthenticationUpdated()
		{
			if(!ViewModel.IsUserValid())
			{
				App.AuthUserProfile = null;
				await DisplayAlert("Invalid Email", "This service is only available to Xamarin employees.", "OK");
			}
			else
			{
				await Navigation.PushAsync(new AdminPage());
			}

			_userLabel.Text = App.AuthUserProfile == null ? "empty" : App.AuthUserProfile.Email;
			_loginButton.IsVisible = App.AuthUserProfile == null;
			_logoutButton.IsVisible = !_loginButton.IsVisible;
		}

		async public Task CheckForAuthentication()
		{
			await ViewModel.GetUserProfile();

			if(App.AuthUserProfile != null)
			{
				await ViewModel.EnsureAthleteRegistered();
			}
			else
			{
				MessagingCenter.Send<AuthenticationPage>(this, "AuthenticateUser");
			}

			UserAuthenticationUpdated();
		}

		void Initialize()
		{
			BackgroundColor = Color.White;

			_activity = new ActivityIndicator();
			_activity.SetBinding(ActivityIndicator.IsRunningProperty, "IsBusy");
			_activity.SetBinding(ActivityIndicator.IsVisibleProperty, "IsBusy");
			_activity.Color = Color.Gray;

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
						_userLabel,
					_loginButton,
						_logoutButton
				}
			};
		}
	}
}