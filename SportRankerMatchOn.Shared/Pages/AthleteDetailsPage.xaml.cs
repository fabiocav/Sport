﻿using Xamarin.Forms;
using System.Threading.Tasks;

namespace SportRankerMatchOn.Shared
{
	public partial class AthleteDetailsPage : AthleteDetailsXaml
	{
		public AthleteDetailsPage(Athlete league = null)
		{
			ViewModel.Athlete = league ?? new Athlete();
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
				var accepted = await DisplayAlert("Delete Athlete?", "Are you totes sure you want to delete this athlete?", "Yeah", "Nah");

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
				await Navigation.PushAsync(new AthleteLeagueAssociationsPage());	
			};
		}

		protected override void OnAppearing()
		{
			name.Focus();
			base.OnAppearing();
		}
	}

	public partial class AthleteDetailsXaml : BaseContentPage<AthleteDetailsViewModel>
	{
	}
}