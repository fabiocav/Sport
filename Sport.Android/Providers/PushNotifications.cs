using System;
using Android.App;
using Android.Content;
using Gcm.Client;
using Newtonsoft.Json;
using Sport.Shared;
using Xamarin.Forms;
using Android.OS;
using Xamarin;

[assembly: Permission(Name = "@PACKAGE_NAME@.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "@PACKAGE_NAME@.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "com.google.android.c2dm.permission.RECEIVE")]
[assembly: UsesPermission(Name = "android.permission.WAKE_LOCK")]
//GET_ACCOUNTS is only needed for android versions 4.0.3 and below
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

				//Call to Register the device for Push Notifications
				GcmClient.Register(Forms.Context, GcmBroadcastReceiver.SENDER_IDS);
			}
			catch(Exception e)
			{
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

	[IntentFilter(new string[] {
		"Android.Content.Intent.ActionBootCompleted"
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
		
			// Extract the push notification message from the intent.
			string message = null;
			string payload = null;
			if(intent.Extras.ContainsKey("message"))
			{
				message = intent.Extras.Get("message").ToString();
				var title = intent.Extras.Get("title").ToString();

				var pintent = PendingIntent.GetActivity(context, 0, new Intent(context, typeof(MainActivity)), 0);

				var n = new Notification.Builder(context);
				n.SetSmallIcon(Resource.Drawable.ic_successstatus);
				n.SetContentIntent(pintent);
				n.SetContentTitle(title);
				n.SetTicker(message);
				n.SetSmallIcon(Resource.Drawable.icon);
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
			}
		
			sWakeLock.Release();
		}
	}

	[Service] //Must use the service tag
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
				Console.WriteLine(e);
			}
		}

		protected override void OnUnRegistered(Context context, string registrationId)
		{
			//Receive notice that the app no longer wants notifications
		}

		protected override void OnMessage(Context context, Intent intent)
		{
			string message = null;
			string payload = null;

			// Extract the push notification message from the intent.
			if(intent.Extras.ContainsKey("message"))
			{
				message = intent.Extras.Get("message").ToString();
				var title = intent.Extras.Get("title").ToString();

				var n = new Notification.Builder(context);
				n.SetSmallIcon(Android.Resource.Drawable.ic_successstatus);
				n.SetContentTitle(title);
				n.SetTicker(message);
				n.SetSmallIcon(Android.Resource.Drawable.icon);
				n.SetLargeIcon(global::Android.Graphics.BitmapFactory.DecodeResource(Resources, Resource.Drawable.icon));
				n.SetContentText(message);

				var nm = NotificationManager.FromContext(context);
				nm.Notify(0, n.Build());

				Device.BeginInvokeOnMainThread(() =>
				{
					message.ToToast(ToastNotificationType.Info, "Incoming notification");
				});

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
		}

		protected override bool OnRecoverableError(Context context, string errorId)
		{
			//Some recoverable error happened
			return base.OnRecoverableError(context, errorId);
		}

		protected override void OnError(Context context, string errorId)
		{
			//Some more serious error happened
		}
	}
}