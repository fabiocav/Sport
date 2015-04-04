using System;
using Xamarin.Forms;
using Toasts.Forms.Plugin.Abstractions;

namespace SportChallengeMatchRank.Shared
{
	public class MatchResultsFormPage : BaseContentPage<MatchResultsFormViewModel>
	{
		public Action OnMatchResultsPosted
		{
			get;
			set;
		}

		public MatchResultsFormPage(Challenge challenge)
		{
			ViewModel.Challenge = challenge;
			Initialize();
		}

		protected override void Initialize()
		{
			Title = "Post Match Results";
			var scrollView = new ScrollView();

			var stackLayout = new StackLayout {
				Spacing = 10,
				Padding = 10,
			};


			var btnSubmit = new Button {
				Text = "Post Match Result",
				Style = (Style)App.Current.Resources["buttonStyle"],
			};

			var btnCancel = new ToolbarItem {
				Text = "Cancel"
			};
			ToolbarItems.Add(btnCancel);

			btnCancel.Clicked += async(sender, e) =>
			{
				await Navigation.PopModalAsync();		
			};

			for(int i = 0; i < ViewModel.Challenge.League.MatchGameCount; i++)
			{
				var gameResult = new GameResult();
				gameResult.ChallengeId = ViewModel.Challenge.Id;
				ViewModel.Challenge.GameResults.Add(gameResult);

				var form = new GameResultFormView(ViewModel.Challenge, gameResult, i);
				stackLayout.Children.Add(form);
			}

			scrollView.Content = stackLayout;
			stackLayout.Children.Add(btnSubmit);
			Content = scrollView;

			btnSubmit.Clicked += async(sender, e) =>
			{
				bool submit = await DisplayAlert("This will end the match", "Are you sure you want to submit these scores?", "Yes", "No");

				if(submit)
				{
					await ViewModel.PostMatchResults();
					Navigation.PopModalAsync();

					if(OnMatchResultsPosted != null)
						OnMatchResultsPosted();

					var notificator = DependencyService.Get<IToastNotificator>();
					var message = "Your match results have been submitted. Congrats to {0}".Fmt(ViewModel.Challenge.WinningAthlete.Email);
					await notificator.Notify(ToastNotificationType.Success, "Submitted", message, TimeSpan.FromSeconds(2));
				}
			};
		}
	}
}