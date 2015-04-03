using Xamarin.Forms;
using System.Threading.Tasks;
using System;

namespace SportChallengeMatchRank.Shared
{
	public partial class AthleteEditPage : AthleteEditXaml
	{
		public AthleteEditPage(Athlete member = null)
		{
			ViewModel.Athlete = member ?? new Athlete();
		}

		protected override void Initialize()
		{
			InitializeComponent();
			Title = "Edit Athlete";

			var btnCancel = new ToolbarItem {
				Text = "Cancel"
			};

			btnSaveAthlete.Clicked += async(sender, e) =>
			{
				await SaveAthlete();
			};

			btnCancel.Clicked += async(sender, e) =>
			{
				await Navigation.PopModalAsync();		
			};

			ToolbarItems.Add(btnCancel);

			btnDeleteAthlete.Clicked += async(sender, e) =>
			{
				await DeleteAthlete();
			};

			btnMemberStatus.Clicked += async(sender, e) =>
			{
				await Navigation.PushAsync(new MembershipsLandingPage(ViewModel.Athlete));	
			};
		}

		public Action OnUpdate
		{
			get;
			set;
		}

		async Task SaveAthlete()
		{
			await ViewModel.SaveAthlete();

			if(OnUpdate != null)
				OnUpdate();

			await Navigation.PopModalAsync();
		}

		async Task DeleteAthlete()
		{
			var accepted = await DisplayAlert("Delete Athlete?", "Are you totes sure you want to delete this athlete?", "Yes", "No");

			if(accepted)
			{
				await ViewModel.DeleteAthlete();

				if(OnUpdate != null)
					OnUpdate();

				await Navigation.PopModalAsync();
			}
		}

		protected override void OnAppearing()
		{
			if(ViewModel.Athlete.Id == null)
				name.Focus();

			base.OnAppearing();
		}
	}

	public partial class AthleteEditXaml : BaseContentPage<AthleteEditViewModel>
	{
	}
}