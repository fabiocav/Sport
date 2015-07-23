using System;
using System.Threading.Tasks;
using Sport.Shared;
using UIKit;
using Xamarin.Auth;
using Xamarin.Forms;

[assembly: Dependency(typeof(Sport.iOS.AuthenticationProvider))]

namespace Sport.iOS
{
	public class AuthenticationProvider : IAuthentication
	{
		public async Task<Tuple<string, string>> AuthenticateUser()
		{
			return await Task.Run(() =>
			{
				var tcs = new TaskCompletionSource<Tuple<string, string>>();

				var auth = new OAuth2Authenticator(Keys.GoogleApiClientId, Keys.GoogleClientSecret, Keys.GoogleOAuthScope, new Uri(Keys.GoogleOAuthAuthUrl), new Uri(Keys.GoogleOAthRedirectUrl), new Uri(Keys.GoogleOAuthTokenUrl));
				auth.AllowCancel = true;
				auth.ShowUIErrors = false;
				auth.ClearCookiesBeforeLogin = false;

				auth.Completed += (sender, e) =>
				{
					Device.BeginInvokeOnMainThread(() => UIApplication.SharedApplication.KeyWindow.RootViewController.DismissViewController(true, null));

					Tuple<string, string> result = null;
					if(e.IsAuthenticated)
					{
						result = new Tuple<string, string>(e.Account.Properties["access_token"], e.Account.Properties["refresh_token"]);
					}

					tcs.TrySetResult(result);
				};
					
				Device.BeginInvokeOnMainThread(() =>
				{
					UIViewController vc = auth.GetUI();
					UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(vc, true, null);
				});

				return tcs.Task;
			});
		}
	}
}

