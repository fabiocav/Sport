using Xamarin.UITest;
using Xamarin.UITest.Configuration;
using Xamarin.UITest.Utils;
using System;

namespace Sport.UITests
{
	public class AppInitializer
	{
		public static IApp StartApp(Platform platform)
		{
			if(platform == Platform.Android)
			{
				return ConfigureApp.Android.ApiKey(Keys.XamarinTestCloudApiKey).WaitTimes(new DefaultWaitTimes()).StartApp();
				//	return ConfigureApp.Android.ApiKey(Constants.ApiKey).ApkFile("/Users/Rob/Desktop/com.xamarin.sport.apk").StartApp();
			}

			return ConfigureApp.iOS.ApiKey(Keys.XamarinTestCloudApiKey).StartApp(AppDataMode.Clear);
		}
	}

	class DefaultWaitTimes : IWaitTimes
	{
		public TimeSpan GestureWaitTimeout
		{
			get
			{
				return TimeSpan.FromMinutes(5);
			}
		}

		public TimeSpan WaitForTimeout
		{
			get
			{
				return TimeSpan.FromMinutes(5);
			}
		}
	}
}