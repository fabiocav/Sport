﻿using System;
using Xamarin.Forms;
using System.Threading.Tasks;
using SportRankerMatchOn.Shared.Mobile;

namespace SportRankerMatchOn.Shared.Mobile
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
		}

		async public void UserAuthenticationUpdated()
		{
			if(!ViewModel.IsUserValid())
			{
				AppSettings.AuthUserProfile = null;
//				await DisplayAlert("Invalid Email", "This service is only available to Xamarin employees.", "OK");
			}
			else
			{
//				await Task.Delay(2000);
//				await Navigation.PushAsync(new AdminPage());
			}

			_userLabel.Text = AppSettings.AuthUserProfile == null ? "empty" : AppSettings.AuthUserProfile.Email;
			_loginButton.IsVisible = AppSettings.AuthUserProfile == null;
			_logoutButton.IsVisible = !_loginButton.IsVisible;
		}

		async protected override void OnAppearing()
		{
			await ViewModel.GetUserProfile();
			UserAuthenticationUpdated();

			base.OnAppearing();
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
					_loginButton, _logoutButton }
			};
		}
	}
}