using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using SportRankerMatchOn.Shared;

namespace SportRankerMatchOn.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : FormsApplicationDelegate
	{
		UIWindow window;

		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			Forms.Init();

			window = new UIWindow(UIScreen.MainScreen.Bounds);
			//window.RootViewController = new OAuthViewController();
			//window.MakeKeyAndVisible();

			//return true;
			LoadApplication(new App());
			return base.FinishedLaunching(app, options);
		}
	}
}

