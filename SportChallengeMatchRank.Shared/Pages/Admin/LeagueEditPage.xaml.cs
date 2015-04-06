using Xamarin.Forms;
using System.Threading.Tasks;
using System.Linq;
using System;
using Toasts.Forms.Plugin.Abstractions;

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
						"It's on like a prawn that yawns at dawn!".ToToast(ToastNotificationType.Success, "League Started!");
					}
				}
				catch(Exception ex)
				{
					ex.Message.ToToast(ToastNotificationType.Error, "Unable to start league ");
				}
			};
			
			btnSaveLeague.Clicked += async(sender, e) =>
			{
				await SaveLeague();
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
				"The league name '{0}' is already in use. Please choose another.".Fmt(ViewModel.League.Name).ToToast(ToastNotificationType.Warning, "League name in use");
				ViewModel.League.Name = string.Empty;
				name.Focus();
			}
			else if(result == SaveLeagueResult.Failed)
			{
				"There was an error saving this league. Take the rest of the day off.".ToToast(ToastNotificationType.Error);
			}
		}

		async public void OnSelectPhotoButtonClicked(object sender, EventArgs e)
		{
			var photoPage = new PhotoSelectionPage(ViewModel.League);
			photoPage.OnImageSelected = async() =>
			{
				ViewModel.OnPropertyChanged("League");		
				await Navigation.PopAsync();
				photoPage.OnImageSelected = null;
			};

			await Navigation.PushAsync(photoPage);
		}
	}

	public partial class LeagueEditXaml : BaseContentPage<LeagueEditViewModel>
	{
	}
}