using Xamarin.Forms;
using System;

namespace SportChallengeMatchRank.Shared
{
	public partial class AthleteChallengesPage : AthleteChallengesXaml
	{
		public AthleteChallengesPage(string athleteId)
		{
			ViewModel.AthleteId = athleteId;
			Initialize();
		}

		protected override void Initialize()
		{
			InitializeComponent();
			Title = "My Challenges";

			list.ItemSelected += async(sender, e) =>
			{
				if(list.SelectedItem == null)
					return;

				var challenge = list.SelectedItem as Challenge;
				list.SelectedItem = null;
				var detailsPage = new ChallengeDetailsPage(challenge);
				detailsPage.OnDecline = () =>
				{
					Device.BeginInvokeOnMainThread(() =>
					{
						ViewModel.LocalRefresh();
					});
				};

				await Navigation.PushAsync(detailsPage);
			};
		}

		protected override void OnAppearing()
		{
			ViewModel.SetPropertyChanged("ChallengeGroups");
			base.OnAppearing();
		}
	}

	public partial class AthleteChallengesXaml : BaseContentPage<AthleteChallengesViewModel>
	{
	}
}