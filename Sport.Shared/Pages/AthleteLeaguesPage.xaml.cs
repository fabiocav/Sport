using Xamarin.Forms;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Sport.Shared
{
	public partial class AthleteLeaguesPage : AthleteLeaguesXaml
	{
		bool _hasAuthenticated;

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
				//page.ViewModel.Leagues?.Clear();

				page.OnJoinedLeague = (l) =>
				{
					ViewModel.LocalRefresh();
					ViewModel.SetPropertyChanged("Athlete");
				};

				await Navigation.PushModalAsync(page.GetNavigationPage());
			};

			//list.ButtonStyle = (Style)App.Current.Resources["actionButtonStyle"];
			list.OnRankings = async(league) =>
			{
				if(!league.HasStarted)
				{
					"This league hasn't started".ToToast();
					return;
				}

				var membershipsPage = new LeaderboardPage(league);
				await Navigation.PushAsync(membershipsPage);	
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
				//using(new HUD("Getting leagues..."))
				{
					await ViewModel.RemoteRefresh();
					ViewModel.Leagues.Select(vm => vm.League).ToList().ForEach(App.Current.GetTheme);
				}
			}
		}

		async protected override void OnAppearing()
		{
			base.OnAppearing();
			await EnsureUserAuthenticated();
		}

		protected override async void OnUserAuthenticated()
		{
			base.OnUserAuthenticated();
			ViewModel.AthleteId = App.CurrentAthlete.Id;

			if(App.CurrentAthlete != null)
			{
				ViewModel.LocalRefresh();
				ViewModel.Leagues.Select(vm => vm.League).ToList().ForEach(App.Current.GetTheme);
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

		public async Task EnsureUserAuthenticated()
		{
			if(App.CurrentAthlete == null && !_hasAuthenticated)
			{
				_hasAuthenticated = true;

				var authPage = new AuthenticationPage();
				await Navigation.PushModalAsync(authPage);
				await authPage.AttemptToAuthenticateAthlete();

				if(App.CurrentAthlete != null)
				{
					await Navigation.PopModalAsync();
				}
			}
		}

		const string _admin = "Admin";
		const string _profile = "My Profile";
		const string _logout = "Log Out";

		List<string> GetMoreMenuOptions()
		{
			var list = new List<string>();
			list.Add(_profile);

			if(App.CurrentAthlete.IsAdmin)
				list.Add(_admin);

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
		}

		async void OnLogoutSelected()
		{
			var decline = await DisplayAlert("Really?", "Are you sure you want to log out?", "Yes", "No");

			if(!decline)
				return;

			_hasAuthenticated = false;
			var authViewModel = DependencyService.Get<AuthenticationViewModel>();
			authViewModel.LogOut();

			App.Current.SetToWelcomePage(); 
		}

		async void OnProfileSelected()
		{
			var page = new AthleteProfilePage(App.CurrentAthlete.Id);
			page.OnSave = async() => await Navigation.PopModalAsync();
			await Navigation.PushModalAsync(page.GetNavigationPage());
		}

		async void OnAdminSelected()
		{
			var admin = new NavigationPage(new AdminPage());
			await Navigation.PushModalAsync(admin);
		}
	}

	public partial class AthleteLeaguesXaml : BaseContentPage<AthleteLeaguesViewModel>
	{
	}
}