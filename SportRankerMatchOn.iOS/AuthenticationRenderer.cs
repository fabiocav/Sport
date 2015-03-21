using System;
using System.Threading.Tasks;
using Auth0.SDK;
using SportRankerMatchOn.Shared;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly:ExportRenderer(typeof(AuthenticationPage), typeof(SportRankerMatchOn.iOS.AuthenticationRenderer))]

namespace SportRankerMatchOn.iOS
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
					AppSettings.AuthToken = user.IdToken;
					App.AuthUserProfile = profile;
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
			MessagingCenter.Subscribe<BaseViewModel>(this, "AuthenticateUser", async(sender) =>
				{
					await AuthenticateUser();
					_page.UserAuthenticationUpdated();
				});
			
			base.ViewDidAppear(animated);
		}

		public override void ViewDidDisappear(bool animated)
		{
			MessagingCenter.Unsubscribe<BaseViewModel>(this, "AuthenticateUser");
			base.ViewDidDisappear(animated);
		}

		async protected override void OnElementChanged(VisualElementChangedEventArgs e)
		{
			_page = e.NewElement as AuthenticationPage;
			base.OnElementChanged(e);

			if(_page != null)
			{
			}
		}
	}
}