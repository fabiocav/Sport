using Xamarin.Forms;
using System;

namespace SportChallengeMatchRank.Shared
{
	public partial class AthleteChallengesPage : AthleteChallengesXaml
	{
		public AthleteChallengesPage(Athlete athlete)
		{
			ViewModel.Athlete = athlete;
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
				detailsPage.OnDelete = () =>
				{
					App.CurrentAthlete.RefreshChallenges();
				};

				await Navigation.PushAsync(detailsPage);
			};
		}
	}

	public partial class AthleteChallengesXaml : BaseContentPage<AthleteChallengesViewModel>
	{
	}
}