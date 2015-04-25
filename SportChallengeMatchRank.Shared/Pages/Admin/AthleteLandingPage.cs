using Xamarin.Forms;

namespace SportChallengeMatchRank.Shared
{
	public partial class AthleteLandingPage : AthleteLandingXaml
	{
		public AthleteLandingPage()
		{
			Initialize();
		}

		async protected override void Initialize()
		{
			InitializeComponent();
			Title = "Athletes";

			list.ItemSelected += async(sender, e) =>
			{
				if(list.SelectedItem == null)
					return;

				var athlete = list.SelectedItem as Athlete;
				list.SelectedItem = null;

				var detailsPage = new AthleteProfilePage(athlete.Id);
				detailsPage.OnSave = () =>
				{
					ViewModel.LocalRefresh();
					detailsPage.OnSave = null;
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