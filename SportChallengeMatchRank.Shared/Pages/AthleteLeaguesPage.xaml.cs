using Xamarin.Forms;
using System;

namespace SportChallengeMatchRank.Shared
{
	public partial class AthleteLeaguesPage : AthleteLeaguesXaml
	{
		public AthleteLeaguesPage()
		{
			Initialize();
		}

		protected override void Initialize()
		{
			Title = "My Leagues";
			InitializeComponent();

			btnJoin.Clicked += async(sender, e) =>
			{
				var page = new AvailableLeaguesPage();
				page.OnJoinedLeague = (l) =>
				{
					ViewModel.SetPropertyChanged("Athlete");
				};

				await Navigation.PushModalAsync(new NavigationPage(page));
			};
			
			list.ItemSelected += async(sender, e) =>
			{
				if(list.SelectedItem == null)
					return;

				var league = list.SelectedItem as League;
				list.SelectedItem = null;

				var page = new LeagueDetailsPage(league);
				page.OnAbandondedLeague = (l) =>
				{
					ViewModel.SetPropertyChanged("Athlete");
				};
					
				await Navigation.PushAsync(page);
			};
		}

		async protected override void OnUserAuthenticated()
		{
			base.OnUserAuthenticated();
			await ViewModel.GetLeagues();
		}
	}

	public partial class AthleteLeaguesXaml : BaseContentPage<AthleteLeaguesViewModel>
	{
	}
}