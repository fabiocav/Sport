using System;
using System.Threading.Tasks;
using Sport.Shared;
using UIKit;
using Xamarin.Auth;
using Xamarin.Forms;
using System.Text.RegularExpressions;
using System.Linq;

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

				var auth = new OAuth2Authenticator(Constants.GoogleApiClientId, Constants.GoogleClientSecret, Constants.GoogleOAuthScope, new Uri(Constants.GoogleOAuthAuthUrl), new Uri(Constants.GoogleOAthRedirectUrl), new Uri(Constants.GoogleOAuthTokenUrl));
				auth.AllowCancel = true;

				auth.Completed += (sender, e) =>
				{
					UIApplication.SharedApplication.KeyWindow.RootViewController.DismissViewController(true, null);
					tcs.TrySetResult(null);
				};

				auth.PageLoaded += async(sender, e) =>
				{
					Console.WriteLine(auth.DocumentTitle);
					if(auth.DocumentTitle != null && auth.DocumentTitle.StartsWith("Success "))
					{
						var param = auth.DocumentTitle.Split(' ')[1];
						var keys = Regex.Matches(param, "([^?=&]+)(=([^&]*))?").Cast<Match>().ToDictionary(x => x.Groups[1].Value, x => x.Groups[3].Value);

						Device.BeginInvokeOnMainThread(() =>
						{
							UIApplication.SharedApplication.KeyWindow.RootViewController.DismissViewController(true, null);
						});

						var vm = new BaseViewModel();
						var task = GoogleApiService.Instance.GetAuthAndRefreshToken(keys["code"]);
						await vm.RunSafe(task);

						tcs.TrySetResult(task.Result);
					}
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

