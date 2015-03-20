using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SportRankerMatchOn.Shared.Mobile;
using Xamarin.Forms;

namespace SportRankerMatchOn.Shared.Mobile
{
	public class AuthenticationViewModel : BaseViewModel
	{
		public bool IsUserValid()
		{
			return AppSettings.AuthUserProfile != null &&
			AppSettings.AuthUserProfile.Email != null &&
			AppSettings.AuthUserProfile.Email.EndsWith("@xamarin.com", StringComparison.OrdinalIgnoreCase) &&
			AppSettings.AuthUserProfile.EmailVerified;
		}

		public void LogOut()
		{
			AppSettings.AuthToken = null;
			AppSettings.AuthUserID = null;
			AppSettings.AuthUserProfile = null;
		}

		async public Task GetUserProfile(bool force = false)
		{
			if(!force && (AppSettings.AuthUserProfile != null || string.IsNullOrWhiteSpace(AppSettings.AuthToken) || string.IsNullOrWhiteSpace(AppSettings.AuthUserID)))
				return;

			using(new Busy(this))
			{
				try
				{
					string json = null;
					using(var client = new HttpClient())
					{
						client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer {0}".Fmt(AppSettings.AuthToken));
						json = await client.GetStringAsync("https://{0}.com/api/users/{1}".Fmt(Constants.AuthDomain, AppSettings.AuthUserID));
					}

					if(json != null)
					{
						AppSettings.AuthUserProfile = JsonConvert.DeserializeObject<UserProfile>(json);
					}
				}
				catch(Exception e)
				{
					Console.WriteLine("Error getting user profile: {0}", e);
				}
			}
		}
	}
}