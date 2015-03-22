using System;
using System.Windows.Input;
using Xamarin.Forms;
using System.Threading.Tasks;

[assembly: Dependency(typeof(SportRankerMatchOn.Shared.AdminViewModel))]

namespace SportRankerMatchOn.Shared
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