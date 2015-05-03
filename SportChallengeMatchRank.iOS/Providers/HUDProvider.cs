using BigTed;
using SportChallengeMatchRank.Shared;
using Xamarin.Forms;

[assembly: Dependency(typeof(SportChallengeMatchRank.iOS.HUDProvider))]

namespace SportChallengeMatchRank.iOS
{
	public class HUDProvider : IHUDProvider
	{
		public void DisplayProgress(string message)
		{
			if(string.IsNullOrWhiteSpace(message))
			{
				BTProgressHUD.Show(null, -1, ProgressHUD.MaskType.Black);
			}
			else
			{
				BTProgressHUD.Show(message, -1, ProgressHUD.MaskType.Black);
			}
		}

		public void DisplaySuccess(string message)
		{
			BTProgressHUD.ShowSuccessWithStatus(message);
		}

		public void DisplayError(string message)
		{
			BTProgressHUD.ShowErrorWithStatus(message);
		}

		public void Dismiss()
		{
			BTProgressHUD.Dismiss();
		}
	}
}