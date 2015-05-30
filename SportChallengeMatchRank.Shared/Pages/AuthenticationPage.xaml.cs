using System;
using Toasts.Forms.Plugin.Abstractions;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace SportChallengeMatchRank.Shared
{
	public partial class AuthenticationPage : AuthenticationPageXaml
	{
		public AuthenticationPage()
		{
			Initialize();
		}

		protected override void Initialize()
		{
			InitializeComponent();
			Title = "Authenticating...";
		}

		async public Task AttemptToAuthenticateAthlete(bool force = false)
		{
			AuthenticationViewModel.OnDisplayAuthForm = (url) => Device.BeginInvokeOnMainThread(() =>
			{
				//App.Current.Hud.Dismiss();                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      
				var webView = new WebView {
					Source = url			
				};

				var page = new ContentPage {
					Content = webView
				};

				Navigation.PushModalAsync(page);
			});

			AuthenticationViewModel.OnHideAuthForm = async() => await Navigation.PopModalAsync();

			//await Task.Delay(10000);
			await AuthenticationViewModel.GetUserProfile(force);
			if(App.AuthUserProfile != null)
			{
				await AuthenticationViewModel.EnsureAthleteRegistered();
			}
		}
	}

	public partial class AuthenticationPageXaml : BaseContentPage<AuthenticationViewModel>
	{
	}
}