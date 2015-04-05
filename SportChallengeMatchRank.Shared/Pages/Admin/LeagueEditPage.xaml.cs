using Xamarin.Forms;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace SportChallengeMatchRank.Shared
{
	public partial class LeagueEditPage : LeagueEditXaml
	{
		public Action OnUpdate
		{
			get;
			set;
		}

		public LeagueEditPage(League league = null)
		{
			ViewModel.League = league ?? new League();
			Initialize();
		}

		protected override void Initialize()
		{
			InitializeComponent();
			Title = "Edit League";

			btnStartLeague.Clicked += async(sender, e) =>
			{
				try
				{
					var startTime = await ViewModel.StartLeague();

					if(startTime != null)
					{
						btnStartLeague.IsVisible = false;
						await DisplayAlert("League started!", "It's on like a prawn that yawns at dawn!", "Mkay");
					}
				}
				catch(Exception ex)
				{
					await DisplayAlert("Unable to start league", ex.Message, "Mkay");
				}
			};
			
			btnSaveLeague.Clicked += async(sender, e) =>
			{
				var photoPage = new PhotoSelectionPage(ViewModel.League);
				photoPage.OnImageSelected = async() =>
				{
					await Navigation.PopAsync();
					await SaveLeague();
				};
					
				await Navigation.PushAsync(photoPage);
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
				var accepted = await DisplayAlert("Delete League?", "Are you totes sure you want to delete this league?", "Yes", "No");

				if(accepted)
				{
					await ViewModel.DeleteLeague();

					if(OnUpdate != null)
						OnUpdate();
					
					await Navigation.PopModalAsync();
				}
			};
		}

		protected override void OnAppearing()
		{
			if(ViewModel.League.Id == null)
				name.Focus();

			ViewModel.UpdateMembershipStatus();
			base.OnAppearing();
		}

		async Task SaveLeague()
		{
			if(!ViewModel.IsValid())
			{
				errorView.Opacity = 0.0;
				errorView.IsVisible = true;
				await errorView.FadeTo(1.0);
				await Task.Delay(2000);
				await errorView.FadeTo(0.0);
				errorView.IsVisible = false;
				return;
			}

			ViewModel.League.ImageUrl = "https://dl.dropboxusercontent.com/u/54307520/ping-pong-page-banner.jpg";
			var result = await ViewModel.SaveLeague();

			if(result == SaveLeagueResult.OK)
			{
				DataManager.Instance.Leagues.AddOrUpdate(ViewModel.League);

				if(OnUpdate != null)
					OnUpdate();

				await Navigation.PopModalAsync();
			}
			else if(result == SaveLeagueResult.Conflict)
			{
				await DisplayAlert("Name in Use", "The league name '{0}' is already in use. Please choose another.".Fmt(ViewModel.League.Name), "OK");
				ViewModel.League.Name = string.Empty;
				name.Focus();
			}
			else if(result == SaveLeagueResult.Failed)
			{
				await DisplayAlert("Unable to Save League", "There was an error saving this league. Take the rest of the day off.", "OK");
			}
		}
	}

	public partial class LeagueEditXaml : BaseContentPage<LeagueEditViewModel>
	{
	}
}