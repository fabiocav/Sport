using Xamarin.UITest;

namespace Sport.UITests
{
	public class AppInitializer
	{
		public static IApp StartApp(Platform platform)
		{
			if(platform == Platform.Android)
			{
				//return ConfigureApp.Android.ApiKey(Constants.ApiKey).StartApp();
				return ConfigureApp.Android.ApiKey(Constants.ApiKey).ApkFile("/Users/Rob/Desktop/com.xamarin.sport.apk").StartApp();
			}

			//return ConfigureApp.iOS.ApiKey(Constants.ApiKey).StartApp();
			return ConfigureApp.iOS.ApiKey(Constants.ApiKey).InstalledApp("com.xamarin.sport").StartApp();
		}
	}
}