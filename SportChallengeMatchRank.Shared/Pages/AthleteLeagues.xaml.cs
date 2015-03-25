using Xamarin.Forms;

namespace SportChallengeMatchRank.Shared
{
	public partial class AthleteLeaguesPage : AthleteLeaguesXaml
	{
		async protected override void Initialize()
		{
			InitializeComponent();
			Title = "My Leagues";

			btnJoin.Clicked += async(sender, e) =>
			{
				await Navigation.PushModalAsync(new NavigationPage(new AvailableLeaguesPage()));
			};
			
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

		LeagueEditPage GetDetailsPage(League league)
		{
			var detailsPage = new LeagueEditPage(league);
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