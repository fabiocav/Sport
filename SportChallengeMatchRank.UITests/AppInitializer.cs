using System;
using System.IO;
using System.Linq;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace SportChallengeMatchRank.UITests
{
	public class AppInitializer
	{
		public static IApp StartApp(Platform platform)
		{
			if(platform == Platform.Android)
			{
				return ConfigureApp.Android.ApiKey(Constants.ApiKey).StartApp();
			}

			return ConfigureApp.iOS.ApiKey(Constants.ApiKey).StartApp();
		}
	}
}

