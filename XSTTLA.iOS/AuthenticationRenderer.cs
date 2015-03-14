
using System;
using UIKit;
using XSTTLA.Shared;
using Auth0.SDK;
using System.Threading.Tasks;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;

[assembly:ExportRenderer(typeof(AuthenticationPage), typeof(XSTTLA.iOS.AuthenticationRenderer))]

namespace XSTTLA.iOS
{
	public class AuthenticationRenderer : PageRenderer
	{
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

		public override void ViewDidAppear(bool animated)
		{
			MessagingCenter.Subscribe<AuthenticationPage>(this, "AuthenticateUser", async(sender) =>
			{
				await AuthenticateUser();
				sender.UserAuthenticationUpdated();
			});
			base.ViewDidAppear(animated);
		}

		public override void ViewDidDisappear(bool animated)
		{
			MessagingCenter.Unsubscribe<AuthenticationPage>(this, "AuthenticateUser");
			base.ViewDidDisappear(animated);
		}

		protected override void OnElementChanged(VisualElementChangedEventArgs e)
		{
			var page = e.NewElement as AuthenticationPage;

			if(page != null)
			{
			}

			base.OnElementChanged(e);
		}
	}
}