using Xamarin.Forms;
using System;

namespace SportChallengeMatchRank.Shared
{
	public partial class AthleteChallengesPage : AthleteChallengesXaml
	{
		public AthleteChallengesPage(string athleteId = null)
		{
			ViewModel.AthleteId = athleteId;
			Initialize();
		}

		protected async override void Initialize()
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
					ViewModel.LocalRefresh();
				};

				detailsPage.OnAccept = () =>
				{
					ViewModel.LocalRefresh();
				};

				detailsPage.OnPostResults = () =>
				{
					ViewModel.LocalRefresh();
				};

				await Navigation.PushAsync(detailsPage);
			};

			if(App.CurrentAthlete != null)
				await ViewModel.GetChallenges();
		}

		protected override void OnAppearing()
		{
			ViewModel.LocalRefresh();
			ViewModel.SetPropertyChanged("ChallengeGroups");
			base.OnAppearing();
		}

		protected async override void OnUserAuthenticated()
		{
			base.OnUserAuthenticated();
			ViewModel.AthleteId = App.CurrentAthlete.Id;
			await ViewModel.GetChallenges();
		}
	}

	public partial class AthleteChallengesXaml : BaseContentPage<AthleteChallengesViewModel>
	{
	}
}