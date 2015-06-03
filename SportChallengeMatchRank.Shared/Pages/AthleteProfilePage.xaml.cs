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
					"Profile saved".ToToast(ToastNotificationType.Success);

					if(OnSave != null)
						OnSave();
				}
			};

			AddDoneButton();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			ViewModel.Athlete.RefreshChallenges();
			ViewModel.Athlete.RefreshMemberships();
		}

		//		protected override async void OnIncomingPayload(App app, NotificationPayload payload)
		//		{
		//			string leagueId;
		//			if(payload.Payload.TryGetValue("leagueId", out leagueId))
		//			{
		//				if(leagueId == ViewModel.League.Id)
		//				{
		//					await ViewModel.RefreshLeague();
		//				}
		//			}
		//
		//			string challengeId;
		//			if(payload.Payload.TryGetValue("challengeId", out challengeId))
		//			{
		//				await ViewModel.RefreshLeague();
		//			}
		//		}
	}

	public partial class AthleteProfilePageXaml : BaseContentPage<AthleteProfileViewModel>
	{
	}
}