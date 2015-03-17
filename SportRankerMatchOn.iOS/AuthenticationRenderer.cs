using System;
using System.Threading.Tasks;
using Auth0.SDK;
using SportRankerMatchOn.Shared;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using SportRankerMatchOn.Shared.Mobile;

[assembly:ExportRenderer(typeof(AuthenticationPage), typeof(SportRankerMatchOn.iOS.AuthenticationRenderer))]

namespace SportRankerMatchOn.iOS
{
	public class AuthenticationRenderer : PageRenderer
	{
		AuthenticationPage _page;

		async public Task AuthenticateUser()
		{
			if(AppSettings.AuthUserProfile == null)
			{
				Auth0User user;
				try
				{
					var authClient = new Auth0Client(Constants.AuthDomain, Constants.AuthClientId);
					authClient.Logout();
					user = await authClient.LoginAsync(this, Constants.AuthType, true);
					var profile = user.Profile.ToObject<UserProfile>();
					AppSettings.AuthToken = user.IdToken;
					AppSettings.AuthUserProfile = profile;
					AppSettings.AuthUserID = profile.UserId;
				}
				catch(Exception e)
				{
					Console.WriteLine("Error authenticating user: {0}" + e);	
				}
			}
		}

		async public override void ViewDidAppear(bool animated)
		{
			await AuthenticateUser();
			_page.UserAuthenticationUpdated();

//			MessagingCenter.Subscribe<AuthenticationPage>(this, "AuthenticateUser", async(sender) =>
//			{
//				await AuthenticateUser();
//				sender.UserAuthenticationUpdated();
//			});
			base.ViewDidAppear(animated);
		}

		public override void ViewDidDisappear(bool animated)
		{
			MessagingCenter.Unsubscribe<AuthenticationPage>(this, "AuthenticateUser");
			base.ViewDidDisappear(animated);
		}

		protected override void OnElementChanged(VisualElementChangedEventArgs e)
		{
			_page = e.NewElement as AuthenticationPage;

			if(_page != null)
			{
			}

			base.OnElementChanged(e);
		}
	}
}