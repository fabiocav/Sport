using System;
using System.Threading.Tasks;

namespace SportChallengeMatchRank.Shared
{
	public interface IPushNotifications
	{
		bool IsRegistered
		{
			get;
		}

		Task RegisterForPushNotifications();
	}
}