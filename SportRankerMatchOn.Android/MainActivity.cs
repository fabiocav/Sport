using Android.App;
using Android.OS;
using SportRankerMatchOn.Shared.Mobile;
using Xamarin.Forms.Platform.Android;

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