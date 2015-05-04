using System;
using Foundation;
using ImageCircle.Forms.Plugin.iOS;
using Toasts.Forms.Plugin.iOS;
using UIKit;
using Xamarin.Forms;
using XLabs.Forms;
using SportChallengeMatchRank.Shared;
using Toasts.Forms.Plugin.Abstractions;

namespace SportChallengeMatchRank.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : XFormsApplicationDelegate
	{
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			Forms.Init();
			ImageCircleRenderer.Init();
			ToastNotificatorImplementation.Init();

			UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.LightContent, false);
			LoadApplication(new App());

			UITabBar.Appearance.BarTintColor = UIColor.FromRGB(44, 62, 80);
			UITabBar.Appearance.TintColor = UIColor.White;

			UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;

//			UINavigationBar.Appearance.BackgroundColor = UIColor.Clear;
//			UINavigationBar.Appearance.SetBackgroundImage(new UIImage(), UIBarMetrics.Default);
//			UINavigationBar.Appearance.ShadowImage = new UIImage();

			return base.FinishedLaunching(app, options);
		}

		public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
		{
			App.CurrentAthlete.DeviceToken = deviceToken.Description.Trim('<', '>').Replace(" ", "");
			MessagingCenter.Send<App>(App.Current, "RegisteredForRemoteNotifications");
		}

		public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
		{
			Console.WriteLine("FailedToRegisterForRemoteNotifications called");
			Console.WriteLine(error);
		}

		public override void DidRegisterUserNotificationSettings(UIApplication application, UIUserNotificationSettings notificationSettings)
		{
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
			NSObject aps;
			NSObject alert;

			bool success = userInfo.TryGetValue(new NSString("aps"), out aps);
			success = ((NSDictionary)aps).TryGetValue(new NSString("alert"), out alert);

			var badgeValue = ((NSDictionary)aps).ObjectForKey(new NSString("badge"));

			if(badgeValue != null)
			{
				int count;
				if(int.TryParse(new NSString(badgeValue.ToString()), out count))
				{
					//UIApplication.SharedApplication.ApplicationIconBadgeNumber = count;
				}
			}

			if(success)
			{
				alert.ToString().ToToast(ToastNotificationType.Info, "Incoming notification");
			}
		}
	}
}