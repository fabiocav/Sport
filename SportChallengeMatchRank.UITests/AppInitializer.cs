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
				return ConfigureApp.Android.ApiKey("a835baddbb619daf8c09f1e49756e81f").EnableLocalScreenshots().StartApp();
			}

			return ConfigureApp.iOS.ApiKey("a835baddbb619daf8c09f1e49756e81f").StartApp();
		}
	}
}

