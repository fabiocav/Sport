using System;
using Xamarin.Forms;

namespace XSTTLA.Shared
{
	public class App : Application
	{
		public App()
		{
			MainPage = new NavigationPage(new AdminPage());
		}
	}
}