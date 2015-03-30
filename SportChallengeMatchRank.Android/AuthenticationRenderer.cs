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
		AuthenticationPage _page;

		async public Task AuthenticateUser()
		{
			if(App.AuthUserProfile == null)
			{
				var authClient = new Auth0Client(Constants.AuthDomain, Constants.AuthClientId);
					
				if(Settings.Instance.AuthToken != null && Settings.Instance.RefreshToken != null)
				{
					var vm = DependencyService.Get<AuthenticationViewModel>();
					var result = authClient.GetDelegationToken("app", Settings.Instance.AuthToken, Settings.Instance.RefreshToken, Constants.AuthClientId);
					await vm.GetUserProfile(true);
				}


				Auth0User user;
				try
				{
					authClient.Logout();
					user = await authClient.LoginAsync(this.Context, Constants.AuthType, true);
					var profile = user.Profile.ToObject<UserProfile>();
					App.AuthUserProfile = profile;
					Settings.Instance.AuthUserID = profile.UserId;
					Settings.Instance.AuthToken = user.IdToken;
					Settings.Instance.RefreshToken = user.RefreshToken;
					await Settings.Instance.Save();
				}
				catch(Exception e)
				{
					Console.WriteLine("Error authenticating user: {0}" + e);	
				}
			}
		}

		protected override void OnAttachedToWindow()
		{
			MessagingCenter.Subscribe<AuthenticationViewModel>(this, "AuthenticateUser", async(sender) =>
				{
					await AuthenticateUser();
					await _page.UserAuthenticationUpdated();
				});

			base.OnAttachedToWindow();
		}

		protected override void OnDetachedFromWindow()
		{
			MessagingCenter.Unsubscribe<AuthenticationPage>(this, "AuthenticateUser");
			base.OnDetachedFromWindow();
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
		{
			_page = e.NewElement as AuthenticationPage;
			base.OnElementChanged(e);
		}
	}
}