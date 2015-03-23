using Xamarin.Forms;

namespace SportChallengeMatchRank.Shared
{
	public partial class MembershipsLandingPage : MembershipsLandingXaml
	{
		public MembershipsLandingPage(Athlete athlete)
		{
			ViewModel.Athlete = athlete;
			Initialize();
		}

		public MembershipsLandingPage(League league)
		{
			ViewModel.League = league;
			Initialize();
		}

		void Initialize()
		{
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
			if(ViewModel.Athlete != null)
			{
				await ViewModel.GetAllMembershipsByAthlete();
				list.SetBinding(ListView.ItemsSourceProperty, "Athlete.Memberships");
			}

			if(ViewModel.League != null)
			{
				await ViewModel.GetAllMembershipsByLeague();
				list.SetBinding(ListView.ItemsSourceProperty, "League.Memberships");
			}

			base.OnLoaded();
		}
	}

	public partial class MembershipsLandingXaml : BaseContentPage<MembershipsLandingViewModel>
	{
	}
}