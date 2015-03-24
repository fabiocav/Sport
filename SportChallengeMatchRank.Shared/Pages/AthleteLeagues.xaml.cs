using Xamarin.Forms;

namespace SportChallengeMatchRank.Shared
{
	public partial class AthleteLeaguesPage : AthleteLeaguesXaml
	{
		public AthleteLeaguesPage()
		{
			InitializeComponent();
			Initialize();
		}

		async void Initialize()
		{
			Title = "My Leagues";

			list.ItemSelected += async(sender, e) =>
			{
				if(list.SelectedItem == null)
					return;

				var league = list.SelectedItem as League;
				list.SelectedItem = null;
				await Navigation.PushModalAsync(new NavigationPage(GetDetailsPage(league)));
			};

			await ViewModel.GetLeagues();
		}

		LeagueDetailsPage GetDetailsPage(League league)
		{
			var detailsPage = new LeagueDetailsPage(league);
			detailsPage.OnUpdate = () =>
			{
				ViewModel.LocalRefresh();
				detailsPage.OnUpdate = null;
			};

			return detailsPage;
		}
	}

	public partial class AthleteLeaguesXaml : BaseContentPage<AthleteLeaguesViewModel>
	{
	}
}