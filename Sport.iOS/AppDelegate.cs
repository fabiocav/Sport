using System;
using Foundation;
using ImageCircle.Forms.Plugin.iOS;
using Newtonsoft.Json;
using Sport.Shared;
using UIKit;
using Xamarin;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace Sport.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : FormsApplicationDelegate
	{
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			Insights.Initialize(Keys.InsightsApiKey);

			//#if DEBUG
			Xamarin.Calabash.Start();
			//#endif

			Forms.Init();
			ImageCircleRenderer.Init();
			ToastNotifier.Init();
		
			Forms.ViewInitialized += (sender, e) =>
			{
				if(null != e.View.StyleId)
				{
					e.NativeView.AccessibilityIdentifier = e.View.StyleId;
				}
			};

			var atts = new UITextAttributes {
				Font = UIFont.FromName("SegoeUI", 22),
			};
			UINavigationBar.Appearance.SetTitleTextAttributes(atts);

			var barButtonAtts = new UITextAttributes {
				Font = UIFont.FromName("SegoeUI", 16),
			};

			UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.LightContent;
			UIBarButtonItem.Appearance.SetTitleTextAttributes(barButtonAtts, UIControlState.Normal);
			UIBarButtonItem.Appearance.SetBackButtonTitlePositionAdjustment(new UIOffset(0, -100), UIBarMetrics.Default);

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
			MessagingCenter.Send<App>(App.Current, "RegisteredForRemoteNotifications");
		}

		public override void DidRegisterUserNotificationSettings(UIApplication application, UIUserNotificationSettings notificationSettings)
		{
			Console.WriteLine("DidRegisterUserNotificationSettings called");
		}

		public override void HandleAction(UIApplication application, string actionIdentifier, NSDictionary remoteNotificationInfo, Action completionHandler)
		{
			Console.WriteLine("HandleAction called");
		}

		public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
		{
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