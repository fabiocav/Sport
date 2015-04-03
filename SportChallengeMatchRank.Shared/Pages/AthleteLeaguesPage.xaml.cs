using Xamarin.Forms;
using System;

namespace SportChallengeMatchRank.Shared
{
	public partial class AthleteLeaguesPage : AthleteLeaguesXaml
	{
		public AthleteLeaguesPage()
		{
			Title = "My Leagues";
		}

		protected override void Initialize()
		{
			InitializeComponent();

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
				await Navigation.PushAsync(new LeagueDetailsPage(league));
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