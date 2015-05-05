using Xamarin.Forms;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
			{
				RemoteRefresh();
			}
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
			await RemoteRefresh();
		}

		protected override async void OnIncomingPayload(App app, NotificationPayload payload)
		{
			if(payload.Action.StartsWith("Challenge"))
			{
				await RemoteRefresh();
			}
		}

		async Task RemoteRefresh()
		{
			await ViewModel.GetChallenges(true);
			ViewModel.LocalRefresh();
			ViewModel.SetPropertyChanged("ChallengeGroups");
		}
	}

	public partial class AthleteChallengesXaml : BaseContentPage<AthleteChallengesViewModel>
	{
	}
}