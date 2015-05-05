using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace SportChallengeMatchRank.Shared
{
	public interface IPushNotifications
	{
		//event EventHandler<IncomingPushNotificationEventArgs> OnPushNotificationReceived;

		bool IsRegistered
		{
			get;
		}

		Task RegisterForPushNotifications();

	}

	public class IncomingPushNotificationEventArgs : EventArgs
	{
		public Dictionary<string, object> Payload
		{
			get;
			set;
		}
	}
}