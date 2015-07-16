using Xamarin.Forms;

[assembly: Dependency(typeof(Sport.Shared.AdminViewModel))]

namespace Sport.Shared
{
	public class AdminViewModel : BaseViewModel
	{
		public void LogOut()
		{
			Settings.Instance.AthleteId = null;
			Settings.Instance.AuthToken = null;
			Settings.Instance.RefreshToken = null;
			Settings.Instance.Save();
		}
	}
}