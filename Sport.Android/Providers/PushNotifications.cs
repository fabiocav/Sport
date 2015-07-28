using System;
using Android.App;
using Android.Content;
using Android.OS;
using Gcm.Client;
using Newtonsoft.Json;
using Sport.Shared;
using Xamarin;
using Xamarin.Forms;

[assembly: Permission(Name = "@PACKAGE_NAME@.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "@PACKAGE_NAME@.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "com.google.android.c2dm.permission.RECEIVE")]
[assembly: UsesPermission(Name = "android.permission.WAKE_LOCK")]

[assembly: UsesPermission(Name = "android.permission.GET_ACCOUNTS")]
[assembly: UsesPermission(Name = "android.permission.INTERNET")]
[assembly: UsesPermission(Name = "android.permission.WAKE_LOCK")]

[assembly: Dependency(typeof(Sport.Android.PushNotifications))]

namespace Sport.Android
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

		public void RegisterForPushNotifications()
		{
			try
			{
				GcmClient.CheckDevice(Forms.Context);
				GcmClient.CheckManifest(Forms.Context);
				GcmClient.Register(Forms.Context, GcmBroadcastReceiver.SENDER_IDS);
			}
			catch(Exception e)
			{
				Insights.Report(e);
				Console.WriteLine(e);
			}
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
		public static string[] SENDER_IDS = {
			Keys.GooglePushNotificationSenderId
		};

		public override void OnReceive(Context context, Intent intent)
		{
			PowerManager.WakeLock sWakeLock;
			var pm = PowerManager.FromContext(context);
			sWakeLock = pm.NewWakeLock(WakeLockFlags.Partial, "GCM Broadcast Reciever Tag");
			sWakeLock.Acquire();

			if(!HandlePushNotification(context, intent))
			{
				base.OnReceive(context, intent);
			}
		
			sWakeLock.Release();
		}

		internal static bool HandlePushNotification(Context context, Intent intent)
		{
			string message;
			string payload;
			if(!intent.Extras.ContainsKey("message"))
				return false;

			message = intent.Extras.Get("message").ToString();
			var title = intent.Extras.Get("title").ToString();

			var activityIntent = new Intent(context, typeof(MainActivity));
			activityIntent.SetFlags(ActivityFlags.SingleTop);
			var pintent = PendingIntent.GetActivity(context, 0, activityIntent, PendingIntentFlags.UpdateCurrent);

			var n = new Notification.Builder(context);
			n.SetSmallIcon(Resource.Drawable.ic_successstatus);
			n.SetContentIntent(pintent);
			n.SetContentTitle(title);
			n.SetTicker(message);
			n.SetLargeIcon(global::Android.Graphics.BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.icon));
			n.SetSmallIcon(Resource.Drawable.ic_trophy_white);
			n.SetContentText(message);

			var nm = NotificationManager.FromContext(context);
			nm.Notify(0, n.Build());

			if(MainActivity.IsRunning)
			{
				try
				{
					message.ToToast();
					if(intent.Extras.ContainsKey("payload"))
					{
						payload = intent.Extras.Get("payload").ToString();
						var payloadValue = JsonConvert.DeserializeObject<NotificationPayload>(payload);

						if(payloadValue != null)
						{
							Device.BeginInvokeOnMainThread(() =>
							{
								MessagingCenter.Send<App, NotificationPayload>(App.Current, "IncomingPayloadReceived", payloadValue);
							});
						}
					}
				}
				catch(Exception e)
				{
					Insights.Report(e, Insights.Severity.Error);
				}
			}

			return true;
		}
	}

	[Service]
	public class PushHandlerService : GcmServiceBase
	{
		public PushHandlerService() : base(GcmBroadcastReceiver.SENDER_IDS)
		{
		}

		protected override void OnRegistered(Context context, string registrationId)
		{
			try
			{
				App.CurrentAthlete.DeviceToken = registrationId;
				MessagingCenter.Send<App>(App.Current, "RegisteredForRemoteNotifications");
			}
			catch(Exception e)
			{
				Insights.Report(e);
				Console.WriteLine(e);
			}
		}

		protected override void OnMessage(Context context, Intent intent)
		{
			GcmBroadcastReceiver.HandlePushNotification(context, intent);
		}

		protected override void OnUnRegistered(Context context, string registrationId)
		{
			//Receive notice that the app no longer wants notifications
		}

		protected override void OnError(Context context, string errorId)
		{
			//Some more serious error happened
			Console.WriteLine("PushHandlerService error: " + errorId);
		}
	}
}