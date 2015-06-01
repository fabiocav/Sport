using System;
using Xamarin.Forms;
using System.Threading.Tasks;

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

		async protected override void Initialize()
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
					"Profile saved".ToToast(ToastNotificationType.Success);

					if(OnSave != null)
						OnSave();
				}
			};

			var btnCancel = new ToolbarItem {
				Text = "Done",
			};

			btnCancel.Clicked += async(sender, e) =>
			{
				await Navigation.PopModalAsync();		
			};

			ToolbarItems.Add(btnCancel);
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