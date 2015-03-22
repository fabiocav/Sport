using Xamarin.Forms;

namespace SportRankerMatchOn.Shared
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
				await Navigation.PushModalAsync(new NavigationPage(new AthleteDetailsPage(league)));
			};
		}

		async protected override void OnLoaded()
		{
			await ViewModel.GetAllAthletes();
			base.OnLoaded();
		}

		protected override void OnAppearing()
		{
			list.ItemsSource = null;
			list.SetBinding(ListView.ItemsSourceProperty, "AllAthletes");

			base.OnAppearing();
		}
	}

	public partial class AthleteLandingXaml : BaseContentPage<AthleteLandingViewModel>
	{
	}
}