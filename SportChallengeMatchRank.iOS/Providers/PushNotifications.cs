using Foundation;
using SportChallengeMatchRank.Shared;
using UIKit;
using Xamarin.Forms;
using System.Threading.Tasks;

[assembly: Dependency(typeof(SportChallengeMatchRank.iOS.PushNotifications))]

namespace SportChallengeMatchRank.iOS
{
	public class PushNotifications : IPushNotifications
	{
		public Task RegisterForPushNotifications()
		{
			return new Task(() =>
			{
				Device.BeginInvokeOnMainThread(() =>
				{
					var settings = UIUserNotificationSettings.GetSettingsForTypes(UIUserNotificationType.Alert
					               | UIUserNotificationType.Badge
					               | UIUserNotificationType.Sound, new NSSet());


					UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
					UIApplication.SharedApplication.RegisterForRemoteNotifications();
				});
			});
		}

		public bool IsRegistered
		{
			get
			{
				return UIApplication.SharedApplication.IsRegisteredForRemoteNotifications;
			}
		}
	}
}

