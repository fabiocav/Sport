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
		}

		//		protected override void OnParentSet()
		//		{
		//			base.OnParentSet();
		//
		//			if(ParentView == null)
		//				return;
		//
		//			var width = ParentView.Bounds.Width / 3;
		//
		//			photoImage.WidthRequest = 200;
		//			photoImage.HeightRequest = width;
		//		}

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