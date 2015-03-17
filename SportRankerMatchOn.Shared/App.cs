using System;
using Xamarin.Forms;

namespace SportRankerMatchOn.Shared
{
	public class App : Application
	{
		public App()
		{
			MainPage = new NavigationPage(new AdminPage());
		}
	}
}