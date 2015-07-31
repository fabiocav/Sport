using System;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace Sport.Shared
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
				if(string.IsNullOrWhiteSpace(ViewModel.Athlete.Alias))
				{
					"Please enter an alias.".ToToast(ToastNotificationType.Warning);
					return;
				}

				bool success;
				using(new HUD("Saving..."))
				{
					success = await ViewModel.SaveAthlete();
				}

				if(success)
				{
					"Your profile has been saved".ToToast(ToastNotificationType.Success);

					if(OnSave != null)
						OnSave();
				}
			};

			btnRegister.Clicked += (sender, e) =>
			{
				ViewModel.RegisterForPushNotifications(async() =>
				{
					await Task.Delay(500);
					"Your device has been registered".ToToast();
				});
			};

			AddDoneButton();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			ViewModel.Athlete.RefreshMemberships();
			ViewModel.Athlete.Memberships.ForEach(m => m.League.RefreshChallenges());
		}
	}

	public partial class AthleteProfilePageXaml : BaseContentPage<AthleteProfileViewModel>
	{
	}
}