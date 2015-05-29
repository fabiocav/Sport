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

	public class HUD : IDisposable
	{
		public HUD(string message)
		{
			App.Current.Hud.DisplayProgress(message);	
		}

		public void Dispose()
		{
			App.Current.Hud.Dismiss();
		}
	}
}

