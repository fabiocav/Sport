using System;
using System.Threading.Tasks;
using Auth0.SDK;
using SportChallengeMatchRank.Shared;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly:ExportRenderer(typeof(AuthenticationPage), typeof(SportChallengeMatchRank.iOS.AuthenticationRenderer))]

namespace SportChallengeMatchRank.iOS
{
	public class AuthenticationRenderer : PageRenderer
	{
		AuthenticationPage _page;

		async public Task AuthenticateUser()
		{
			if(App.AuthUserProfile == null)
			{
				Auth0User user;
				try
				{
					var authClient = new Auth0Client(Constants.AuthDomain, Constants.AuthClientId);
					authClient.Logout();
					user = await authClient.LoginAsync(this, Constants.AuthType, true);
					var profile = user.Profile.ToObject<UserProfile>();
					Settings.Instance.AuthToken = user.IdToken;
					Settings.Instance.AuthUserID = profile.UserId;
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