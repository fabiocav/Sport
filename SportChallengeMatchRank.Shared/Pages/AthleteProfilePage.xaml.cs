using System;

namespace SportChallengeMatchRank.Shared
{
	public partial class AthleteProfilePage : AthleteProfilePageXaml
	{
		public Action OnSave
		{
			get;
			set;
		}

		public AthleteProfilePage(string athleteId)
		{
			ViewModel.AthleteId = athleteId;
			Initialize();
		}

		protected override void Initialize()
		{
			InitializeComponent();
			Title = "Profile";

			btnSave.Clicked += async(sender, e) =>
			{
				bool success;
				using(new HUD("Saving..."))
				{
					success = await ViewModel.SaveAthlete();
				}

				if(success)
				{
					"Saved".ToToast(ToastNotificationType.Success);

					if(OnSave != null)
						OnSave();
				}
			};

			btnDelete.Clicked += async(sender, e) =>
			{
				if(!App.CurrentAthlete.IsAdmin)
					return;

				var confirmed = await DisplayAlert("Delete {0}'s Profile?".Fmt(ViewModel.Athlete.Name), "Are you sure you want to remove this athlete?", "Yes", "No");

				if(!confirmed)
					return;

				bool success;
				using(new HUD("Deleting..."))
				{
					success = await ViewModel.DeleteAthlete();
				}

				if(success)
				{
					"Deleted".ToToast(ToastNotificationType.Success);
					await Navigation.PopAsync();
				}
			};
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			ViewModel.Athlete.RefreshChallenges();
			ViewModel.Athlete.RefreshMemberships();
		}
	}

	public partial class AthleteProfilePageXaml : BaseContentPage<AthleteProfileViewModel>
	{
	}
}