using System;

namespace SportChallengeMatchRank.Shared
{
	public interface IHUDProvider
	{
		void DisplayProgress(string message);

		void DisplaySuccess(string message);

		void DisplayError(string message);

		void Dismiss();
	}
}

