using Xamarin.Forms;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Sport.Shared
{
	public partial class AthleteLeaguesPage : AthleteLeaguesXaml
	{
		public AthleteLeaguesPage(string athleteId = null)
		{
			ViewModel.AthleteId = athleteId;
			Initialize();
		}

		protected async override void Initialize()
		{
			Title = "Leagues";
			InitializeComponent();

			btnJoin.Clicked += async(sender, e) =>
			{
				var page = new AvailableLeaguesPage();

				page.OnJoinedLeague = (l) =>
				{
					ViewModel.LocalRefresh();
					ViewModel.SetPropertyChanged("Athlete");
				};

				await Navigation.PushModalAsync(page.GetNavigationPage());
			};

			list.ItemSelected += async(sender, e) =>
			{
				if(list.SelectedItem == null)
					return;

				var vm = list.SelectedItem as LeagueViewModel;

				list.SelectedItem = null;

				if(vm.League.Id == null)
					return;

				var page = new LeagueDetailsPage(vm.League);
				page.OnAbandondedLeague = async(l) =>
				{
					ViewModel.LocalRefresh();
					ViewModel.SetPropertyChanged("Athlete");
					await Navigation.PopAsync();
				};

				await Navigation.PushAsync(page);
			};

			if(App.CurrentAthlete != null)
			{
				await ViewModel.RemoteRefresh();
			}

			await EnsureUserAuthenticated();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			foreach(var l in ViewModel.Leagues)
				l.LocalRefresh();
		}

		internal override void OnUserAuthenticated()
		{
			base.OnUserAuthenticated();
			ViewModel.AthleteId = App.CurrentAthlete.Id;

			if(App.CurrentAthlete != null)
			{
				ViewModel.LocalRefresh();
			}
		}

		protected override async void OnIncomingPayload(App app, NotificationPayload payload)
		{
			string leagueId = null;
			if(payload.Payload.TryGetValue("leagueId", out leagueId))
			{
				//await ViewModel.RemoteRefresh();
				return;
			}

			string challengeId;
			if(payload.Payload.TryGetValue("challengeId", out challengeId))
			{
				await ViewModel.RemoteRefresh();
				return;
			}
		}

		const string _admin = "Admin";
		const string _profile = "My Profile";
		const string _logout = "Log Out";
		const string _about = "About";

		List<string> GetMoreMenuOptions()
		{
			var list = new List<string>();
			list.Add(_profile);

			if(App.CurrentAthlete.IsAdmin)
				list.Add(_admin);

			list.Add(_about);
			list.Add(_logout);

			return list;
		}

		async void OnMoreClicked(object sender, EventArgs e)
		{
			var list = GetMoreMenuOptions();
			var action = await DisplayActionSheet("Additional actions", "Cancel", null, list.ToArray());

			if(action == _logout)
				OnLogoutSelected();

			if(action == _profile)
				OnProfileSelected();

			if(action == _admin)
				OnAdminSelected();

			if(action == _about)
				OnAboutSelected();
		}

		async void OnLogoutSelected()
		{
			LogoutUser();
		}

		async void OnProfileSelected()
		{
			var page = new AthleteProfilePage(App.CurrentAthlete.Id);
			page.OnSave = async() => await Navigation.PopModalAsync();
			await Navigation.PushModalAsync(page.GetNavigationPage());
		}

		async void OnAdminSelected()
		{
			await Navigation.PushModalAsync(new AdminPage().GetNavigationPage());
		}

		async void OnAboutSelected()
		{
			var page = new AboutPage();
			page.AddDoneButton();
			
			await Navigation.PushModalAsync(page.GetNavigationPage());
		}
	}

	public partial class AthleteLeaguesXaml : BaseContentPage<AthleteLeaguesViewModel>
	{
	}
}