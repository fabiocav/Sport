using System;
using Toasts.Forms.Plugin.Abstractions;

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
				var success = await ViewModel.SaveAthlete();

				if(success)
				{
					"Profile saved!".ToToast(ToastNotificationType.Success, "YES!");

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

				var success = await ViewModel.DeleteAthlete();

				if(success)
				{
					"Profile deleted!".ToToast(ToastNotificationType.Success, "YES!");
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