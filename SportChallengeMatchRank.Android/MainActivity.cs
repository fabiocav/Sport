using Android.App;
using Android.OS;
using Xamarin.Forms.Platform.Android;
using SportChallengeMatchRank.Shared;
using Android.Views;
using Toasts.Forms.Plugin.Droid;

namespace SportChallengeMatchRank.Android
{
	[Activity(Label = "SportChallengeMatchRank.Android", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : FormsApplicationActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			Window.SetSoftInputMode(SoftInput.AdjustPan);
			Xamarin.Forms.Forms.Init(this, bundle);
			ToastNotificatorImplementation.Init();
			LoadApplication(new App());
		}
	}
}