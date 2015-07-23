using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Android.Content;
using Sport.Shared;
using Xamarin.Auth;
using Xamarin.Forms;
using Android.App;

[assembly:Dependency(typeof(Sport.Android.AuthenticationProvider))]

namespace Sport.Android
{
	[Activity(NoHistory = true)]
	public class AuthenticationProvider : IAuthentication
	{
		public async Task<Tuple<string, string>> AuthenticateUser()
		{
			return await Task.Run(() =>
			{
				var tcs = new TaskCompletionSource<Tuple<string, string>>();

				var auth = new OAuth2Authenticator(Keys.GoogleApiClientId, Keys.GoogleClientSecret, Keys.GoogleOAuthScope, new Uri(Keys.GoogleOAuthAuthUrl), new Uri(Keys.GoogleOAthRedirectUrl), new Uri(Keys.GoogleOAuthTokenUrl));
				auth.AllowCancel = true;
				auth.ClearCookiesBeforeLogin = false;
				auth.ShowUIErrors = false;
				auth.ShouldLoadPage = (uri) =>
				{
					if(uri.Host == auth.RedirectUrl.Host && uri.LocalPath == auth.RedirectUrl.LocalPath)
						return false;

					return true;
				};

				auth.Completed += (sender, e) =>
				{
					Tuple<string, string> result = null;
					if(e.IsAuthenticated)
					{
						result = new Tuple<string, string>(e.Account.Properties["access_token"], e.Account.Properties["refresh_token"]);
					}

					tcs.TrySetResult(result);
				};

				var intent = auth.GetUI(Forms.Context);
				//intent.SetFlags(ActivityFlags.NewTask);
				Forms.Context.StartActivity(intent);

				return tcs.Task;
			});
		}
	}
}

