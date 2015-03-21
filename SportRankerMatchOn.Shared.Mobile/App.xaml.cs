using System;
using Xamarin.Forms;

namespace SportRankerMatchOn.Shared.Mobile
{
	public partial class App
	{
		public App()
		{
			InitializeComponent();
			MainPage = new NavigationPage(new AdminPage());
		}

		public static Member CurrentMember
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