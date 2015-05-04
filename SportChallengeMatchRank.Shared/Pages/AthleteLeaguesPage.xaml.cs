using Xamarin.Forms;
using System;

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
			Title = "My Leagues";
			InitializeComponent();

			btnJoin.Clicked += async(sender, e) =>
			{
				var page = new AvailableLeaguesPage();
				page.OnJoinedLeague = (l) =>
				{
					ViewModel.LocalRefresh();
					ViewModel.SetPropertyChanged("Athlete");
				};

				await Navigation.PushModalAsync(new NavigationPage(page) {
				});
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
				await ViewModel.GetLeagues();
		}

		protected override async void OnUserAuthenticated()
		{
			base.OnUserAuthenticated();
			ViewModel.AthleteId = App.CurrentAthlete.Id;

			if(App.CurrentAthlete != null)
				await ViewModel.GetLeagues();
		}
	}

	public partial class AthleteLeaguesXaml : BaseContentPage<AthleteLeaguesViewModel>
	{
	}
}