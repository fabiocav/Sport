using System;
using System.Windows.Input;
using Xamarin.Forms;
using System.Threading.Tasks;

[assembly: Dependency(typeof(SportChallengeMatchRank.Shared.AdminViewModel))]

namespace SportChallengeMatchRank.Shared
{
	public class AdminViewModel : BaseViewModel
	{
		public void LogOut()
		{
			AppSettings.AuthToken = null;
			AppSettings.AuthUserID = null;
			App.AuthUserProfile = null;
		}
	}
}