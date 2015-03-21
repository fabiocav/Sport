using System;
using Xamarin.Forms;

namespace SportRankerMatchOn.Shared
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