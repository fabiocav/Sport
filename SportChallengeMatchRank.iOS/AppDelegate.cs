﻿using System;
using Foundation;
using ImageCircle.Forms.Plugin.iOS;
using Toasts.Forms.Plugin.iOS;
using UIKit;
using Xamarin.Forms;
using XLabs.Forms;
using SportChallengeMatchRank.Shared;
using Toasts.Forms.Plugin.Abstractions;
using Newtonsoft.Json;
using System.Collections.Generic;

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