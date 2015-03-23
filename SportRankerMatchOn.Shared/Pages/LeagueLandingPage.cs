using Xamarin.Forms;

namespace SportRankerMatchOn.Shared
{
	public partial class LeagueLandingPage : LeagueLandingXaml
	{
		bool _dataLoaded;

		public LeagueLandingPage()
		{
			InitializeComponent();
			Title = "Leagues";

			btnAdd.Clicked += async(sender, e) =>
			{
				await Navigation.PushModalAsync(new NavigationPage(new LeagueDetailsPage()));	
			};

			list.ItemSelected += async(sender, e) =>
			{
				if(list.SelectedItem == null)
					return;
					
				var league = list.SelectedItem as League;
				list.SelectedItem = null;
				await Navigation.PushModalAsync(new NavigationPage(new LeagueDetailsPage(league)));
			};
		}

		async protected override void OnLoaded()
		{
			await ViewModel.GetAllLeagues();
			_dataLoaded = true;

			base.OnLoaded();
		}

		protected override void OnAppearing()
		{
			if(_dataLoaded)
			{
				list.ItemsSource = null;
				list.SetBinding(ListView.ItemsSourceProperty, "AllLeagues");
			}

			base.OnAppearing();
		}
	}

	public partial class LeagueLandingXaml : BaseContentPage<LeagueLandingViewModel>
	{
	}
}