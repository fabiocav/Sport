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
		public Task<bool> RegisterForPushNotifications()
		{
			return new Task<bool>(() =>
			{
				var settings = UIUserNotificationSettings.GetSettingsForTypes(UIUserNotificationType.Alert
				               | UIUserNotificationType.Badge
				               | UIUserNotificationType.Sound, new NSSet());

				UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
				UIApplication.SharedApplication.RegisterForRemoteNotifications();

				return UIApplication.SharedApplication.IsRegisteredForRemoteNotifications;
			});
		}
	}
}

