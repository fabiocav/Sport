using Android.App;
using Android.OS;
using Xamarin.Forms.Platform.Android;
using SportRankerMatchOn.Shared;

namespace SportRankerMatchOn.Android
{
	[Activity(Label = "SportRankerMatchOn.Android", MainLauncher = true, Icon = "@drawable/icon")]
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