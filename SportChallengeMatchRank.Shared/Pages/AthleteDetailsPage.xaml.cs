using Xamarin.Forms;
using System.Threading.Tasks;

namespace SportChallengeMatchRank.Shared
{
	public partial class AthleteDetailsPage : AthleteDetailsXaml
	{
		public AthleteDetailsPage(Athlete member = null)
		{
			ViewModel.Athlete = member ?? new Athlete();
			InitializeComponent();
			Title = "Athlete Details";

			btnSaveAthlete.Clicked += async(sender, e) =>
			{
				var isNew = ViewModel.Athlete.Id == null;
				await ViewModel.SaveAthlete();
				var landingVm = DependencyService.Get<AthleteLandingViewModel>();

				if(isNew)
				{
					landingVm.AllAthletes.Add(ViewModel.Athlete);
				}
				else
				{
				}

				await Navigation.PopModalAsync();
			};

			var btnCancel = new ToolbarItem {
				Text = "Cancel"		
			};

			btnCancel.Clicked += async(sender, e) =>
			{
				await Navigation.PopModalAsync();		
			};

			ToolbarItems.Add(btnCancel);

			btnDeleteAthlete.Clicked += async(sender, e) =>
			{
				var accepted = await DisplayAlert("Delete Athlete?", "Are you totes sure you want to delete this athlete?", "Yeah brah!", "Nah");

				if(accepted)
				{
					await ViewModel.DeleteAthlete();
					var landingVm = DependencyService.Get<AthleteLandingViewModel>();
					landingVm.AllAthletes.Remove(ViewModel.Athlete);
					await Navigation.PopModalAsync();
				}
			};

			btnMemberStatus.Clicked += async(sender, e) =>
			{
				await Navigation.PushAsync(new MembershipsLandingPage(ViewModel.Athlete));	
			};
		}

		protected override void OnAppearing()
		{
			if(ViewModel.Athlete.Id == null)
				name.Focus();

			base.OnAppearing();
		}
	}

	public partial class AthleteDetailsXaml : BaseContentPage<AthleteDetailsViewModel>
	{
	}
}