using Xamarin.Forms;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace SportChallengeMatchRank.Shared
{
	public partial class LeagueDetailsPage : LeagueDetailsXaml
	{
		public LeagueDetailsPage(League league = null)
		{
			ViewModel.League = league ?? new League();
			Initialize();
		}

		void Initialize()
		{
			InitializeComponent();
			Title = "League Details";

			btnSaveLeague.Clicked += async(sender, e) =>
			{
				if(!ViewModel.IsValid())
					return;

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

	public partial class LeagueDetailsXaml : BaseContentPage<LeagueDetailsViewModel>
	{
	}
}