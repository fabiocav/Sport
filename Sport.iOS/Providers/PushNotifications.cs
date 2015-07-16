using Foundation;
using Sport.Shared;
using UIKit;
using Xamarin.Forms;
using System.Threading.Tasks;
using System;

[assembly: Dependency(typeof(Sport.iOS.PushNotifications))]

namespace Sport.iOS
{
	public class PushNotifications : IPushNotifications
	{
		public void RegisterForPushNotifications()
		{
			var settings = UIUserNotificationSettings.GetSettingsForTypes(UIUserNotificationType.Alert
			               | UIUserNotificationType.Badge
			               | UIUserNotificationType.Sound, new NSSet());


			UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
			UIApplication.SharedApplication.RegisterForRemoteNotifications();
		}

		public bool IsRegistered
		{
			get
			{
				return UIApplication.SharedApplication.IsRegisteredForRemoteNotifications;
			}
		}

		//public event EventHandler<IncomingPushNotificationEventArgs> OnPushNotificationReceived;
	}
}

