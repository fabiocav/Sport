using System;
using Android.App;
using Android.OS;
using Android.Views;
using ImageCircle.Forms.Plugin.Droid;
using SportChallengeMatchRank.Shared;
using Toasts.Forms.Plugin.Droid;
using Xamarin.Forms.Platform.Android;
using XLabs.Forms;

namespace SportChallengeMatchRank.Android
{
	[Activity(Label = "Sport Challenge", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : XFormsApplicationDroid
	{
		protected override void OnCreate(Bundle bundle)
		{
			Xamarin.Insights.Initialize("34553a125b7e69dcaa66abde0e4c851979252f20", this);

//			AppDomain.CurrentDomain.FirstChanceException += (sender, e) =>
//			{
//				Console.WriteLine(e);
//			};
//
			AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
			{
				Console.WriteLine(e);
			};

			try
			{
				base.OnCreate(bundle);
				Window.SetSoftInputMode(SoftInput.AdjustPan);
				Xamarin.Forms.Forms.Init(this, bundle);
				ImageCircleRenderer.Init();
				ToastNotificatorImplementation.Init();

				Xamarin.Forms.Forms.ViewInitialized += (sender, e) =>
				{
					if(!string.IsNullOrWhiteSpace(e.View.StyleId))
					{
						e.NativeView.ContentDescription = e.View.StyleId;
					}
				};

				LoadApplication(new App());

			}
			catch(Exception e)
			{
				Console.WriteLine(e);
			}
		}
	}
}