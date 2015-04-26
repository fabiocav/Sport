using System;
using System.Threading.Tasks;

namespace SportChallengeMatchRank.Shared
{
	public interface IPushNotifications
	{
		Task<bool>  RegisterForPushNotifications();
	}
}

