using Xamarin.Forms;
using System;

namespace SportChallengeMatchRank.Shared
{
	public partial class AthleteChallengesPage : AthleteChallengesXaml
	{
		protected override void Initialize()
		{
			InitializeComponent();
			Title = "Challenges";

			list.ItemSelected += async(sender, e) =>
			{
				if(list.SelectedItem == null)
					return;

				var challenge = list.SelectedItem as Challenge;
				list.SelectedItem = null;
				var detailsPage = new ChallengeDetailsPage(challenge);
				detailsPage.OnDelete = () =>
				{
					App.CurrentAthlete.RefreshChallenges();
					ViewModel.LocalRefresh();
				};

				await Navigation.PushAsync(detailsPage);
			};
		}

		async protected override void OnUserAuthenticated()
		{
			base.OnUserAuthenticated();
			await ViewModel.GetChallenges();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			if(ViewModel.Challenges.Count != App.CurrentAthlete.Challenges.Count)
				ViewModel.LocalRefresh();
		}
	}

	public partial class AthleteChallengesXaml : BaseContentPage<AthleteChallengesViewModel>
	{
	}
}