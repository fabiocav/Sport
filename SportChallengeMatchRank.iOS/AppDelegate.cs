using System;
using Foundation;
using ImageCircle.Forms.Plugin.iOS;
using Newtonsoft.Json;
using SportChallengeMatchRank.Shared;
using UIKit;
using Xamarin;
using Xamarin.Forms;
using XLabs.Forms;

namespace SportChallengeMatchRank.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : XFormsApplicationDelegate
	{
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			Insights.Initialize("34553a125b7e69dcaa66abde0e4c851979252f20");

			#if DEBUG
			Xamarin.Calabash.Start();
			#endif

			Forms.Init();
			ImageCircleRenderer.Init();
			ToastNotifier.Init();

			//UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.LightContent, false);

			UITabBar.Appearance.BarTintColor = UIColor.FromRGB(44, 62, 80);
			UITabBar.Appearance.TintColor = UIColor.White;

			//UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;

			Forms.ViewInitialized += (sender, e) =>
			{
				if(null != e.View.StyleId)
				{
					e.NativeView.AccessibilityIdentifier = e.View.StyleId;
				}
			};

			LoadApplication(new App());
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
			NSObject payload;

			if(!userInfo.TryGetValue(new NSString("aps"), out aps))
				return;
			
			var apsHash = aps as NSDictionary;

			NotificationPayload payloadValue = null;
			if(apsHash.TryGetValue(new NSString("payload"), out payload))
			{
				payloadValue = JsonConvert.DeserializeObject<NotificationPayload>(payload.ToString());
				if(payloadValue != null)
				{
					MessagingCenter.Send<App, NotificationPayload>(App.Current, "IncomingPayloadReceived", payloadValue);
				}
			}

			var badgeValue = apsHash.ObjectForKey(new NSString("badge"));

			if(badgeValue != null)
			{
				int count;
				if(int.TryParse(new NSString(badgeValue.ToString()), out count))
				{
					//UIApplication.SharedApplication.ApplicationIconBadgeNumber = count;
				}
			}

			if(apsHash.TryGetValue(new NSString("alert"), out alert))
			{
				alert.ToString().ToToast(ToastNotificationType.Info, "Incoming notification");
			}
		}
	}
}