using Xamarin.Forms;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace SportChallengeMatchRank.Shared
{
	public partial class LeagueEditPage : LeagueEditXaml
	{
		public LeagueEditPage(League league = null)
		{
			ViewModel.League = league ?? new League();
		}

		protected override void Initialize()
		{
			InitializeComponent();
			Title = "Edit League";

			btnStartLeague.Clicked += async(sender, e) =>
			{
				var startTime = await ViewModel.StartLeague();

				if(startTime != null)
				{
					btnStartLeague.IsVisible = false;
					DisplayAlert("League started!", "It's on like a prawn that yawns at dawn!", "Mkay");
				}
			};
			
			btnSaveLeague.Clicked += async(sender, e) =>
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
				var accepted = await DisplayAlert("Delete League?", "Are you totes sure you want to delete this league?", "Yeah brah!", "Nah");

				if(accepted)
				{
					await ViewModel.DeleteLeague();

					if(OnUpdate != null)
						OnUpdate();
					
					await Navigation.PopModalAsync();
				}
			};

			btnMemberStatus.Clicked += async(sender, e) =>
			{
				await Navigation.PushAsync(new MembershipsLandingPage(ViewModel.League));	
			};
			
		}

		public Action OnUpdate
		{
			get;
			set;
		}

		protected override void OnAppearing()
		{
			if(ViewModel.League.Id == null)
				name.Focus();

			ViewModel.UpdateMembershipStatus();
			base.OnAppearing();
		}
	}

	public partial class LeagueEditXaml : BaseContentPage<LeagueEditViewModel>
	{
	}
}