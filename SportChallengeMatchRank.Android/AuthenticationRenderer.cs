using System;
using Auth0.SDK;
using SportChallengeMatchRank.Shared;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using System.Threading.Tasks;

[assembly:ExportRenderer(typeof(AuthenticationPage), typeof(SportChallengeMatchRank.Android.AuthenticationRenderer))]

namespace SportChallengeMatchRank.Android
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
					Settings.Instance.AuthUserID = profile.UserId;
					Settings.Instance.AuthToken = user.IdToken;
					Settings.Instance.Save();
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
					await sender.UserAuthenticationUpdated();
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