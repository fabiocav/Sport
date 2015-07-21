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
	public class AuthenticationProvider : Activity, IAuthentication
	{
		public async Task<Tuple<string, string>> AuthenticateUser()
		{
			return await Task.Run(() =>
			{
				var tcs = new TaskCompletionSource<Tuple<string, string>>();

				var auth = new OAuth2Authenticator(Constants.GoogleApiClientId, Constants.GoogleClientSecret, Constants.GoogleOAuthScope, new Uri(Constants.GoogleOAuthAuthUrl), new Uri(Constants.GoogleOAthRedirectUrl), new Uri(Constants.GoogleOAuthTokenUrl));
				auth.AllowCancel = true;

				auth.Completed += (sender, e) =>
				{
					tcs.TrySetResult(null);
				};

				auth.PageLoaded += (sender, e) =>
				{
					Console.WriteLine(auth.DocumentTitle);
					if(auth.DocumentTitle != null && auth.DocumentTitle.StartsWith("Success ", StringComparison.Ordinal))
					{
						var param = auth.DocumentTitle.Split(' ')[1];
						var keys = Regex.Matches(param, "([^?=&]+)(=([^&]*))?").Cast<Match>().ToDictionary(x => x.Groups[1].Value, x => x.Groups[3].Value);

						var task = GoogleApiService.Instance.GetAuthAndRefreshToken(keys["code"]);
						task.Start();
						task.Wait();

						tcs.TrySetResult(task.Result);
						Finish(); //this gets hit but never actually does anything - help!!
					}
				};

				var intent = auth.GetUI(Forms.Context);
				intent.SetFlags(ActivityFlags.NewTask);
				Forms.Context.StartActivity(intent);

				return tcs.Task;
			});
		}
	}
}

