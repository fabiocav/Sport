using Android.App;
using Android.OS;
using Xamarin.Forms.Platform.Android;
using SportChallengeMatchRank.Shared;
using Android.Views;
using Toasts.Forms.Plugin.Droid;
using Xamarin.ActionbarSherlockBinding.App;
using ImageCircle.Forms.Plugin.Droid;
using System;

namespace SportChallengeMatchRank.Android
{
	[Activity(Label = "SportChallengeMatchRank.Android", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : FormsApplicationActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
//			AppDomain.CurrentDomain.FirstChanceException += (sender, e) =>
//			{
//				Console.WriteLine(e);
//			};
//
//			AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
//			{
//				Console.WriteLine(e);
//			};

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