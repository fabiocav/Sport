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
		NavigationPage _profilePage;

		public MasterDetailPage()
		{
			_tabbedPage = new AthleteTabbedPage();
			_menu = new MenuPage();
			_menu.ListView.ItemSelected += (sender, e) =>
			{
				if(e.SelectedItem == null)
					return;

				var kvp = (KeyValuePair<string, ICommand>)e.SelectedItem;
				_menu.ListView.SelectedItem = null;
				IsPresented = false;

				kvp.Value.Execute(null);
			};

			MessagingCenter.Subscribe<AuthenticationViewModel>(this, "UserAuthenticated", (viewModel) =>
			{
				AddMenuItems();
			});

			AddMenuItems();

			Master = _menu;
			Detail = _tabbedPage;
		}

		void AddMenuItems()
		{
			var options = new Dictionary<string, ICommand>();
			options.Add("Leagues & Challenges", new Command(() => DisplayLeaguesPage()));
			options.Add("Profile", new Command(() => DisplayProfilePage()));
			//options.Add("Settings", new Command(() => DisplayProfilePage()));
			options.Add("Log Out", new Command(() => LogOutUser()));

			if(App.CurrentAthlete.IsAdmin)
				options.Add("Admin", new Command(() => DisplayAdminPage()));

			_menu.ListView.ItemsSource = options;
		}

		public void DisplayProfilePage()
		{
			_profilePage = _profilePage ?? new NavigationPage(new AthleteProfilePage(App.CurrentAthlete.Id));
			Detail = _profilePage;
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

			//_tabbedPage.EnsureAthleteAuthenticated(true);
		}

		public void DisplayAdminPage()
		{
			_adminPage = _adminPage ?? new NavigationPage(new AdminPage());
			Detail = _adminPage;
		}
	}
}