using System;
using Xamarin.Forms;

namespace SportChallengeMatchRank.Shared
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();
			//MainPage = new AthleteTabbedPage();
			MainPage = new AuthenticationPage();
		}

		public static Athlete CurrentAthlete
		{
			get
			{
				return Settings.Instance.AthleteId == null ? null : DataManager.Instance.Athletes.Get(Settings.Instance.AthleteId);
			}
		}

		public static string DeviceToken
		{
			get;
			set;
		}

		public static UserProfile AuthUserProfile
		{
			get;
			set;
		}
	}
}