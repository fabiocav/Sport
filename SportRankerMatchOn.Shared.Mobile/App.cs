using System;
using Xamarin.Forms;

namespace SportRankerMatchOn.Shared.Mobile
{
	public class App : Application
	{
		public App()
		{
			MainPage = new NavigationPage(new AdminPage());
		}
	}
}