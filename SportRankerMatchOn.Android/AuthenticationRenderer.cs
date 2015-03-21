using System;
using Auth0.SDK;
using SportRankerMatchOn.Shared;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using System.Threading.Tasks;

[assembly:ExportRenderer(typeof(AuthenticationPage), typeof(SportRankerMatchOn.Android.AuthenticationRenderer))]

namespace SportRankerMatchOn.Android
{
	public class AuthenticationRenderer : PageRenderer
	{
		async public Task AuthenticateUser()
		{
			if(App.AuthUserProfile == null)
			{
				Auth0User user;
				try
				{
					var authClient = new Auth0Client(Constants.AuthDomain, Constants.AuthClientId);
					authClient.Logout();
					user = await authClient.LoginAsync(this.Context, Constants.AuthType, true);
					var profile = user.Profile.ToObject<UserProfile>();
					App.AuthUserProfile = profile;
					AppSettings.AuthUserID = profile.UserId;
					AppSettings.AuthToken = user.IdToken;
				}
				catch(Exception e)
				{
					Console.WriteLine("Error authenticating user: {0}" + e);	
				}
			}
		}

		protected override void OnAttachedToWindow()
		{
			MessagingCenter.Subscribe<AuthenticationPage>(this, "AuthenticateUser", async(sender) =>
				{
					await AuthenticateUser();
					sender.UserAuthenticationUpdated();
				});

			base.OnAttachedToWindow();
		}

		protected override void OnDetachedFromWindow()
		{
			MessagingCenter.Unsubscribe<AuthenticationPage>(this, "AuthenticateUser");
			base.OnDetachedFromWindow();
		}
	}
}