using Xamarin.Forms;
using System;
using System.Threading.Tasks;

namespace SportChallengeMatchRank.Shared
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
			PrimaryColor = Color.FromHex("#B3E770");
			PrimaryColorDark = Color.FromHex("#5A8622");

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

				var nav = new NavigationPage(page);
				nav.BarTextColor = Color.White;
				nav.BarBackgroundColor = (Color)App.Current.Resources["bluePrimary"];
				await Navigation.PushModalAsync(nav);
			};

			list.ItemSelected += async(sender, e) =>
			{
				if(list.SelectedItem == null)
					return;

				var league = list.SelectedItem as League;
				list.SelectedItem = null;

				if(league.Id == null)
					return;

				var page = new LeagueDetailsPage(league);

				page.OnAbandondedLeague = (l) =>
				{
					ViewModel.LocalRefresh();
					ViewModel.SetPropertyChanged("Athlete");
				};
					
				await Navigation.PushAsync(page);
			};

			if(App.CurrentAthlete != null)
			{
				//using(new HUD("Getting leagues..."))
				{
					await ViewModel.GetLeagues();
					await ViewModel.GetChallenges();
				}
			}
		}

		protected override async void OnUserAuthenticated()
		{
			base.OnUserAuthenticated();
			ViewModel.AthleteId = App.CurrentAthlete.Id;

			if(App.CurrentAthlete != null)
			{
				//using(new HUD("Getting leagues..."))
				{
					await ViewModel.GetLeagues();
					await ViewModel.GetChallenges();
				}
			}
		}

		protected override async void OnIncomingPayload(App app, NotificationPayload payload)
		{
			string leagueId = null;
			if(payload.Payload.TryGetValue("leagueId", out leagueId))
			{
				await ViewModel.GetLeagues(true);
				await ViewModel.GetChallenges();
			}
		}
	}

	public partial class AthleteLeaguesXaml : BaseContentPage<AthleteLeaguesViewModel>
	{
	}
}