using Xamarin.Forms;

namespace SportChallengeMatchRank.Shared
{
	public partial class AthleteLandingPage : AthleteLandingXaml
	{
		public AthleteLandingPage()
		{
		}

		async protected override void Initialize()
		{
			InitializeComponent();
			Title = "Athletes";

			list.ItemSelected += async(sender, e) =>
			{
				if(list.SelectedItem == null)
					return;

				var league = list.SelectedItem as Athlete;
				list.SelectedItem = null;

				var detailsPage = new AthleteEditPage(league);
				detailsPage.OnUpdate = () =>
				{
					ViewModel.LocalRefresh();
					detailsPage.OnUpdate = null;
				};

				await Navigation.PushModalAsync(new NavigationPage(detailsPage));
			};

			await ViewModel.GetAllAthletes();
		}
	}

	public partial class AthleteLandingXaml : BaseContentPage<AthleteLandingViewModel>
	{
	}
}