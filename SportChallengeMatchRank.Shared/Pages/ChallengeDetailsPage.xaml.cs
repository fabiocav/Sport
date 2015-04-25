using System;
using Toasts.Forms.Plugin.Abstractions;
using Xamarin.Forms;

namespace SportChallengeMatchRank.Shared
{
	public partial class ChallengeDetailsPage : ChallengeDetailsXaml
	{
		public Action OnDecline
		{
			get;
			set;
		}

		public ChallengeDetailsPage(Challenge challenge = null)
		{
			ViewModel.Challenge = challenge ?? new Challenge();
			Initialize();
		}

		async protected override void Initialize()
		{
			InitializeComponent();
			Title = "Challenge Details";

			btnAccept.Clicked += async(sender, e) =>
			{
				var success = await ViewModel.AcceptChallenge();

				if(success)
					"It is sooooooo on like a prawn that yawns at dawn.".ToToast(ToastNotificationType.Success);
			};

			btnPostResults.Clicked += async(sender, e) =>
			{
				var page = new MatchResultsFormPage(ViewModel.Challenge);
				page.OnMatchResultsPosted = () =>
				{
					ViewModel.NotifyPropertiesChanged();
				};

				await Navigation.PushModalAsync(new NavigationPage(page));
			};

			btnRevoke.Clicked += async(sender, e) =>
			{
				var decline = await DisplayAlert("Really?", "Are you sure you want to cowardly revoke this honorable duel?", "Sadly, yes", "No - good point");

				if(!decline)
					return;

				var success = await ViewModel.DeclineChallenge();

				if(success)
					"Unbelievable - how dare you!".ToToast(ToastNotificationType.Info, "Coward!");

				if(OnDecline != null)
					OnDecline();

				await Navigation.PopAsync();
			};

			btnDecline.Clicked += DeclineChallenge;
			btnDeclineAfter.Clicked += DeclineChallenge;

			await ViewModel.GetMatchResults();
		}

		async void DeclineChallenge(object sender, EventArgs e)
		{
			var decline = await DisplayAlert("Really?", "Are you sure you want to cowardly decline this honorable duel?", "Sadly, yes", "No - good point");

			if(!decline)
				return;

			var success = await ViewModel.DeclineChallenge();

			if(success)
				"Unbelievable - how dare you!".ToToast(ToastNotificationType.Info, "Coward!");

			if(OnDecline != null)
				OnDecline();

			await Navigation.PopAsync();
		}
	}

	public partial class ChallengeDetailsXaml : BaseContentPage<ChallengeDetailsViewModel>
	{
	}
}