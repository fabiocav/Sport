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
				detailsPage.OnDelete = () =>
				{
					Device.BeginInvokeOnMainThread(() =>
						{
							App.CurrentAthlete.RefreshChallenges();
							ViewModel.OnPropertyChanged("Athlete");
						});
				};

				await Navigation.PushAsync(detailsPage);
			};
		}

		protected override void OnAppearing()
		{
			ViewModel.OnPropertyChanged("Athlete");
			base.OnAppearing();
		}
	}

	public partial class AthleteChallengesXaml : BaseContentPage<AthleteChallengesViewModel>
	{
	}
}