using Xamarin.Forms;

namespace SportRankerMatchOn.Shared
{
	public partial class MembershipsByAthletePage : MembershipsByAthleteXaml
	{
		public MembershipsByAthletePage(Athlete athlete)
		{
			ViewModel.Athlete = athlete;
			InitializeComponent();
			Title = "Memberships";

			list.ItemSelected += async(sender, e) =>
			{
				if(list.SelectedItem == null)
					return;
					
				var member = list.SelectedItem as Membership;
				list.SelectedItem = null;
				await Navigation.PushModalAsync(new NavigationPage(new MembershipDetailsPage(member)));
			};
		}

		async protected override void OnLoaded()
		{
			await ViewModel.GetAllMembershipsByAthlete();
			base.OnLoaded();
		}

		protected override void OnAppearing()
		{
			list.ItemsSource = null;
			list.SetBinding(ListView.ItemsSourceProperty, "Athlete.Memberships");

			base.OnAppearing();
		}
	}

	public partial class MembershipsByAthleteXaml : BaseContentPage<MembershipsByAthleteViewModel>
	{
	}
}