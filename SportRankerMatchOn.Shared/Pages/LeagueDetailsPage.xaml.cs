﻿using Xamarin.Forms;
using System.Threading.Tasks;

namespace SportRankerMatchOn.Shared
{
	public partial class LeagueDetailsPage : LeagueDetailsXaml
	{
		public LeagueDetailsPage(League league = null)
		{
			ViewModel.League = league ?? new League();
			InitializeComponent();
			Title = "League Details";

			btnSaveLeague.Clicked += async(sender, e) =>
			{
				var isNew = ViewModel.League.Id == null;
				await ViewModel.SaveLeague();
				var landingVm = DependencyService.Get<LeagueLandingViewModel>();

				if(isNew)
				{
					landingVm.AllLeagues.Add(ViewModel.League);
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

			btnDeleteLeague.Clicked += async(sender, e) =>
			{
				var accepted = await DisplayAlert("Delete League?", "Are you totes sure you want to delete this league?", "Yeah", "Nah");

				if(accepted)
				{
					await ViewModel.DeleteLeague();
					var landingVm = DependencyService.Get<LeagueLandingViewModel>();
					landingVm.AllLeagues.Remove(ViewModel.League);
					await Navigation.PopModalAsync();
				}
			};
		}

		protected override void OnAppearing()
		{
			name.Focus();
			base.OnAppearing();
		}
	}

	public partial class LeagueDetailsXaml : BaseContentPage<LeagueDetailsViewModel>
	{
	}
}