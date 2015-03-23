using System;
using Xamarin.Forms;

namespace SportChallengeMatchRank.Shared
{
	public partial class App
	{
		public App()
		{
			InitializeComponent();
			MainPage = new NavigationPage(new AuthenticationPage());
		}

		public static Athlete CurrentAthlete
		{
			get
			{
				return Settings.Instance.AthleteId == null ? null : DataManager.Instance.Athletes.Get(Settings.Instance.AthleteId);
			}
		}

		public static UserProfile AuthUserProfile
		{
			get;
			set;
		}
	}
}