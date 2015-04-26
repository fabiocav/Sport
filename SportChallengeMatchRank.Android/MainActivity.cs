using System;
using Android.App;
using Android.OS;
using Android.Views;
using ImageCircle.Forms.Plugin.Droid;
using SportChallengeMatchRank.Shared;
using Toasts.Forms.Plugin.Droid;
using Xamarin.Forms.Platform.Android;

namespace SportChallengeMatchRank.Android
{
	[Activity(Label = "Sport Challenge", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : FormsApplicationActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
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
				LoadApplication(new App());
			}
			catch(Exception e)
			{
				Console.WriteLine(e);
			}
		}
	}
}