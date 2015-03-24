using Xamarin.Forms;

namespace SportChallengeMatchRank.Shared
{
	public partial class AthleteLandingPage : AthleteLandingXaml
	{
		public AthleteLandingPage()
		{
			InitializeComponent();
			Title = "Athletes";

			list.ItemSelected += async(sender, e) =>
			{
				if(list.SelectedItem == null)
					return;
					
				var league = list.SelectedItem as Athlete;
				list.SelectedItem = null;

				var detailsPage = new AthleteDetailsPage(league);
				detailsPage.OnUpdate = () =>
				{
					ViewModel.LocalRefresh();
					detailsPage.OnUpdate = null;
				};

				await Navigation.PushModalAsync(new NavigationPage(detailsPage));
			};
		}

		async protected override void OnLoaded()
		{
			await ViewModel.GetAllAthletes();
			base.OnLoaded();
		}
	}

	public partial class AthleteLandingXaml : BaseContentPage<AthleteLandingViewModel>
	{
	}
}