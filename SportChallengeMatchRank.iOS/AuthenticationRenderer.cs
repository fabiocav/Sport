using System;
using System.Threading.Tasks;
using Auth0.SDK;
using SportChallengeMatchRank.Shared;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Microsoft.WindowsAzure.MobileServices;
using System.Collections.Generic;

[assembly:ExportRenderer(typeof(AuthenticationPage), typeof(SportChallengeMatchRank.iOS.AuthenticationRenderer))]

namespace SportChallengeMatchRank.iOS
{
	public class AuthenticationRenderer : PageRenderer
	{
		AuthenticationPage _page;

		async public Task AuthenticateUser()
		{
			try
			{
				var args = new Dictionary<string, string>();
				args.Add("scope", "https://www.googleapis.com/auth/plus.profile.emails.read");
				var azureUser = await AzureService.Instance.Client.LoginAsync(this, MobileServiceAuthenticationProvider.Google, args);
				Settings.Instance.AuthToken = azureUser.MobileServiceAuthenticationToken;
				Settings.Instance.AuthUserID = azureUser.UserId.Split(':')[1];
				await Settings.Instance.Save();

				var vm = DependencyService.Get<AuthenticationViewModel>();
				await vm.GetUserProfile(true);
			}
			catch(Exception e)
			{
				Console.WriteLine(e);
			}

			return;

			if(App.AuthUserProfile == null)
			{
				var authClient = new Auth0Client(Constants.AuthDomain, Constants.AuthClientId);

//				if(Settings.Instance.AuthToken != null && Settings.Instance.RefreshToken != null)
//				{
//					var vm = DependencyService.Get<AuthenticationViewModel>();
//					var result = authClient.RenewIdToken();
//					await vm.GetUserProfile(true);
//				}

				Auth0User user;
				try
				{
					authClient.Logout();
					user = await authClient.LoginAsync(this, Constants.AuthType, true);
					var profile = user.Profile.ToObject<UserProfile>();
					Settings.Instance.AuthToken = user.IdToken;
					App.AuthUserProfile = profile;
					await Settings.Instance.Save();
				}
				catch(Exception e)
				{
					Console.WriteLine("Error authenticating user: {0}" + e);	
				}
			}
		}

		public override void ViewDidAppear(bool animated)
		{
			MessagingCenter.Subscribe<AuthenticationViewModel>(this, "AuthenticateUser", async(sender) =>
				{
					await AuthenticateUser();
					await _page.UserAuthenticationUpdated();
				});
			
			base.ViewDidAppear(animated);
		}

		public override void ViewDidDisappear(bool animated)
		{
			MessagingCenter.Unsubscribe<AuthenticationViewModel>(this, "AuthenticateUser");
			base.ViewDidDisappear(animated);
		}

		protected override void OnElementChanged(VisualElementChangedEventArgs e)
		{
			_page = e.NewElement as AuthenticationPage;
			base.OnElementChanged(e);

			if(_page != null)
			{
			}
		}
	}
}