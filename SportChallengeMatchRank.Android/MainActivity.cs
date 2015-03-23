using Android.App;
using Android.OS;
using Xamarin.Forms.Platform.Android;
using SportChallengeMatchRank.Shared;

namespace SportChallengeMatchRank.Android
{
	[Activity(Label = "SportChallengeMatchRank.Android", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : FormsApplicationActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			Xamarin.Forms.Forms.Init(this, bundle);
			LoadApplication(new App());
		}
	}
}