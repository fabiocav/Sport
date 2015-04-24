using System;
using Xamarin.Forms;
using System.Windows.Input;
using System.Collections.Generic;

namespace SportChallengeMatchRank.Shared
{
	public class MasterDetailPage : Xamarin.Forms.MasterDetailPage
	{
		MenuPage _menu;
		NavigationPage _adminPage;
		AthleteTabbedPage _tabbedPage;

		public MasterDetailPage()
		{
			var options = new Dictionary<string, ICommand>();
			options.Add("Leagues", new Command(() => DisplayLeaguesPage()));
			options.Add("Profile", new Command(() => DisplayProfilePage()));
			options.Add("Settings", new Command(() => DisplayProfilePage()));
			options.Add("Admin", new Command(() => DisplayAdminPage()));
			options.Add("Log Out", new Command(() => LogOutUser()));

			_menu = new MenuPage();
			_menu.Options = options;
			_menu.ListView.ItemSelected += (sender, e) =>
			{
				if(e.SelectedItem == null)
					return;

				var kvp = (KeyValuePair<string, ICommand>)e.SelectedItem;
				_menu.ListView.SelectedItem = null;
				IsPresented = false;

				kvp.Value.Execute(null);
			};

			_tabbedPage = new AthleteTabbedPage();

			Master = _menu;
			Detail = _tabbedPage;
		}

		public void DisplayProfilePage()
		{
			
		}

		public void DisplayLeaguesPage()
		{
			Detail = _tabbedPage;
		}

		public void LogOutUser()
		{
			Settings.Instance.AthleteId = null;
			Settings.Instance.AuthToken = null;
			Settings.Instance.RefreshToken = null;
			Settings.Instance.Save();
		}

		public void DisplayAdminPage()
		{
			_adminPage = _adminPage ?? new NavigationPage(new AdminPage());
			Detail = _adminPage;
		}
	}
}