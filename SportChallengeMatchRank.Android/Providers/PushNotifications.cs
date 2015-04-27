using System;
using Android.Content;
using SportChallengeMatchRank.Shared;
using Xamarin.Forms;
using System.Threading.Tasks;

[assembly: Dependency(typeof(SportChallengeMatchRank.Android.PushNotifications))]

namespace SportChallengeMatchRank.Android
{
	public class PushNotifications : IPushNotifications
	{
		public bool IsRegistered
		{
			get
			{
				return true;
			}
		}

		public Task RegisterForPushNotifications()
		{
			return new Task(() =>
			{
			});
		}
	}
}

