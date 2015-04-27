using System;
using Foundation;
using ImageCircle.Forms.Plugin.iOS;
using SportChallengeMatchRank.Shared;
using Toasts.Forms.Plugin.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace SportChallengeMatchRank.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : FormsApplicationDelegate
	{
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			Forms.Init();
			ImageCircleRenderer.Init();
			ToastNotificatorImplementation.Init();

			LoadApplication(new App());

//			UITabBar.Appearance.BarTintColor = UIColor.Red;
//			UITabBar.Appearance.TintColor = UIColor.Orange;

			return base.FinishedLaunching(app, options);
		}

		public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
		{
			MessagingCenter.Send<App>(App.Current, "RegisteredForRemoteNotifications");
			App.DeviceToken = deviceToken.Description.Trim('<', '>').Replace(" ", "");
		}

		public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
		{
			Console.WriteLine("FailedToRegisterForRemoteNotifications called");
			Console.WriteLine(error);
		}

		public override void DidRegisterUserNotificationSettings(UIApplication application, UIUserNotificationSettings notificationSettings)
		{
			MessagingCenter.Send<App>(App.Current, "RegisteredForRemoteNotifications");
			Console.WriteLine("DidRegisterUserNotificationSettings called");
			Console.WriteLine(notificationSettings);
		}

		public override void HandleAction(UIApplication application, string actionIdentifier, NSDictionary remoteNotificationInfo, Action completionHandler)
		{
			Console.WriteLine("HandleAction called");
		}

		public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
		{
			Console.WriteLine(userInfo);
			NSObject inAppMessage;

			bool success = userInfo.TryGetValue(new NSString("inAppMessage"), out inAppMessage);

			if(success)
			{
				var alert = new UIAlertView("Got push notification", inAppMessage.ToString(), null, "OK", null);
				alert.Show();
			}
		}
	}
}