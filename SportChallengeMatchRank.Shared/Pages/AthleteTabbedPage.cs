using System;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace SportChallengeMatchRank.Shared
{
	public class AthleteTabbedPage : TabbedPage
	{
		bool _hasAttemptedAuthentication = false;
		AuthenticationViewModel _viewModel;
		IHUDProvider _hud;

		public AuthenticationViewModel ViewModel
		{
			get
			{
				if(_viewModel == null)
					_viewModel = DependencyService.Get<AuthenticationViewModel>();

				return _viewModel;
			}
		}

		public AthleteTabbedPage()
		{
			_hud = DependencyService.Get<IHUDProvider>();
			var id = App.CurrentAthlete == null ? null : App.CurrentAthlete.Id;

			Children.Add(new NavigationPage(new AthleteLeaguesPage()) {
				Title = "My Leagues",
				//BarBackgroundColor = App.BlueColor
			});
			Children.Add(new NavigationPage(new AthleteChallengesPage(id)) {
				Title = "My Challenges",
				//BarBackgroundColor = App.BlueColor
			});

			Children.Add(new NavigationPage(new AdminPage()) {
				Title = "Admin",
				//BarBackgroundColor = App.BlueColor
			});

			BackgroundColor = App.GreenColor;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			EnsureAthleteAuthenticated();
		}

		#region Authentication

		async public void EnsureAthleteAuthenticated()
		{
			if(App.CurrentAthlete != null)
				return;

			ViewModel.SubscribeToProperty("AuthenticationStatus", () =>
			{
				Console.WriteLine(ViewModel.AuthenticationStatus);
				Device.BeginInvokeOnMainThread(async() =>
				{
					_hud.DisplayProgress(ViewModel.AuthenticationStatus);
					await Task.Delay(1000);
				});
			});

			_hud.DisplayProgress("Authenticating");
			await Task.Delay(200);

			if(App.CurrentAthlete == null && !_hasAttemptedAuthentication)
			{
				_hasAttemptedAuthentication = true;
				await AttemptToAuthenticateAthlete();
				_hud.Dismiss();
			}
		}

		async public Task AttemptToAuthenticateAthlete()
		{
			ViewModel.OnDisplayAuthForm = (url) => Device.BeginInvokeOnMainThread(() =>
			{
				_hud.Dismiss();
				var webView = new WebView();
				webView.Source = url;
				var page = new ContentPage();
				page.Content = webView;
				Navigation.PushModalAsync(page);
			});

			ViewModel.OnHideAuthForm = async() => await Navigation.PopModalAsync();

			await ViewModel.GetUserProfile();
			if(App.AuthUserProfile != null)
			{
				var success = await ViewModel.EnsureAthleteRegistered();
				Console.WriteLine(success);
			}

			if(App.CurrentAthlete != null)
			{
				MessagingCenter.Send<AuthenticationViewModel>(this.ViewModel, "UserAuthenticated");
			}
		}

		#endregion
	}
}