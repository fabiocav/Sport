using System;
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

		public Action OnAccept
		{
			get;
			set;
		}

		public Action OnPostResults
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

			list.ItemSelected += (sender, e) =>
			{
				list.SelectedItem = null;
			};

			btnAccept.Clicked += async(sender, e) =>
			{
				bool success;
				using(new HUD("Accepting..."))
				{
					success = await ViewModel.AcceptChallenge();
				}

				if(success)
					"Accepted".ToToast(ToastNotificationType.Success);

				if(OnAccept != null)
					OnAccept();
			};

			btnPostResults.Clicked += async(sender, e) =>
			{
				var page = new MatchResultsFormPage(ViewModel.Challenge);
				page.OnMatchResultsPosted = () =>
				{
					ViewModel.NotifyPropertiesChanged();

					if(OnPostResults != null)
						OnPostResults();
				};

				await Navigation.PushModalAsync(new NavigationPage(page));
			};

			btnRevoke.Clicked += async(sender, e) =>
			{
				var decline = await DisplayAlert("Really?", "Are you sure you want to cowardly revoke this honorable duel?", "Yes", "No");

				if(!decline)
					return;

				bool success;
				using(new HUD("Revoking challenge..."))
				{
					success = await ViewModel.DeclineChallenge();
				}

				if(success)
					"Revoked".ToToast(ToastNotificationType.Info);

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
			var decline = await DisplayAlert("Really?", "Are you sure you want to cowardly decline this honorable duel?", "Yes", "No");

			if(!decline)
				return;

			bool success;
			using(new HUD("Declining..."))
			{
				success = await ViewModel.DeclineChallenge();
			}

			if(success)
				"Unbelievable!".ToToast(ToastNotificationType.Info);

			if(OnDecline != null)
				OnDecline();

			await Navigation.PopAsync();
		}

		protected override async void OnIncomingPayload(App app, NotificationPayload payload)
		{
			string challengeId = null;
			if(payload.Payload.TryGetValue("challengeId", out challengeId))
			{
				if(challengeId == ViewModel.Challenge.Id && challengeId != null)
				{
					await ViewModel.RefreshChallenge();
				}
			}
		}
	}

	public partial class ChallengeDetailsXaml : BaseContentPage<ChallengeDetailsViewModel>
	{
	}
}