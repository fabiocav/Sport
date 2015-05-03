﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Gcm.Client;
using Microsoft.WindowsAzure.MobileServices;
using SportChallengeMatchRank.Shared;
using Xamarin.Forms;

[assembly: Permission(Name = "@PACKAGE_NAME@.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "@PACKAGE_NAME@.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "com.google.android.c2dm.permission.RECEIVE")]

//GET_ACCOUNTS is only needed for android versions 4.0.3 and below
[assembly: UsesPermission(Name = "android.permission.GET_ACCOUNTS")]
[assembly: UsesPermission(Name = "android.permission.INTERNET")]
[assembly: UsesPermission(Name = "android.permission.WAKE_LOCK")]

[assembly: Dependency(typeof(SportChallengeMatchRank.Android.PushNotifications))]

namespace SportChallengeMatchRank.Android
{
	public class PushNotifications : IPushNotifications
	{
		public bool IsRegistered
		{
			get
			{
				return true;
			}
		}

		public static MobileServiceClient Client
		{
			get;
			private set;
		}

		public Task RegisterForPushNotifications()
		{
			return new Task(() =>
			{
				Client = InternetService.Instance.Client;
				GcmClient.CheckDevice(Forms.Context);
				GcmClient.CheckManifest(Forms.Context);

				//Call to Register the device for Push Notifications
				GcmClient.Register(Forms.Context, GcmBroadcastReceiver.SENDER_IDS);
			});
		}
	}


	[BroadcastReceiver(Permission = Gcm.Client.Constants.PERMISSION_GCM_INTENTS)]
	[IntentFilter(new string[] {
		Gcm.Client.Constants.INTENT_FROM_GCM_MESSAGE
	}, Categories = new string[] {
		"@PACKAGE_NAME@"
	})]
	[IntentFilter(new string[] {
		Gcm.Client.Constants.INTENT_FROM_GCM_REGISTRATION_CALLBACK
	}, Categories = new string[] {
		"@PACKAGE_NAME@"
	})]
	[IntentFilter(new string[] {
		Gcm.Client.Constants.INTENT_FROM_GCM_LIBRARY_RETRY
	}, Categories = new string[] {
		"@PACKAGE_NAME@"
	})]
	public class GcmBroadcastReceiver : GcmBroadcastReceiverBase<PushHandlerService>
	{
		//IMPORTANT: Change this to your own Sender ID!
		//The SENDER_ID is your Google API Console App Project Number
		public static string[] SENDER_IDS = {
			"236481934978"
		};
	}

	[Service] //Must use the service tag
	public class PushHandlerService : GcmServiceBase
	{
		public PushHandlerService() : base(GcmBroadcastReceiver.SENDER_IDS)
		{
		}

		protected override void OnRegistered(Context context, string registrationId)
		{
			// Get the MobileServiceClient from the current activity instance.
			MobileServiceClient client = PushNotifications.Client;           
			var push = client.GetPush();

			List<string> tags = null;

			//// (Optional) Uncomment to add tags to the registration.
			//var tags = new List<string>() { "myTag" }; // create tags if you want

			try
			{
				// Make sure we run the registration on the same thread as the activity, 
				// to avoid threading errors.
				((Activity)Forms.Context).RunOnUiThread(async () => await push.RegisterNativeAsync(registrationId, tags));
				MessagingCenter.Send<App>(App.Current, "RegisteredForRemoteNotifications");
				Debug.WriteLine("The device has been registered with GCM.", registrationId);
				App.DeviceToken = registrationId;
			}
			catch(Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(string.Format("Error with Azure push registration: {0}", ex.Message));                
			}
		}

		protected override void OnUnRegistered(Context context, string registrationId)
		{
			//Receive notice that the app no longer wants notifications
		}

		protected override void OnMessage(Context context, Intent intent)
		{
			string message = string.Empty;

			// Extract the push notification message from the intent.
			if(intent.Extras.ContainsKey("message"))
			{
				message = intent.Extras.Get("message").ToString();
				var title = intent.Extras.Get("title").ToString();

				// Create a notification manager to send the notification.
				//var notificationManager = GetSystemService(Context.NotificationService) as NotificationManager;

				// Create a new intent to show the notification in the UI. 
				//PendingIntent contentIntent = PendingIntent.GetActivity(context, 0, new Intent(this, typeof(MainActivity)), 0);           

				var n = new Notification.Builder(context);
				n.SetSmallIcon(Android.Resource.Drawable.ic_successstatus);
				n.SetContentTitle(title);
				n.SetTicker(message);
				n.SetContentText(message);

				var nm = NotificationManager.FromContext(context);
				nm.Notify(0, n.Build());
			}
		}

		protected override bool OnRecoverableError(Context context, string errorId)
		{
			Console.WriteLine(errorId);
			//Some recoverable error happened
			return base.OnRecoverableError(context, errorId);
		}

		protected override void OnError(Context context, string errorId)
		{
			//Some more serious error happened
		}
	}
}

