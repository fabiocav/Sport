using System;
using Xamarin.Forms;

namespace SportChallengeMatchRank.Shared
{
	public partial class MatchResultsFormPage : MatchResultsFormXaml
	{
		public Action OnMatchResultsPosted
		{
			get;
			set;
		}

		public MatchResultsFormPage(Challenge challenge)
		{
			Title = "Match Score";
			BarBackgroundColor = challenge.League.Theme.Light;
			BarTextColor = challenge.League.Theme.Dark;

			ViewModel.ChallengeId = challenge.Id;
			Initialize();
		}

		protected override void Initialize()
		{
			InitializeComponent();

			var btnCancel = new ToolbarItem {
				Text = "Cancel"
			};
			ToolbarItems.Add(btnCancel);

			btnCancel.Clicked += async(sender, e) =>
			{
				await Navigation.PopModalAsync();		
			};

			ViewModel.Challenge.MatchResult.Clear();
			for(int i = 0; i < ViewModel.Challenge.League.MatchGameCount; i++)
			{
				var gameResult = new GameResult {
					Index = i,
					ChallengeId = ViewModel.Challenge.Id,
				};

				ViewModel.Challenge.MatchResult.Add(gameResult);

				var form = new GameResultFormView(ViewModel.Challenge, gameResult, i);
				games.Children.Add(form);
			}

			btnSubmit.Clicked += async(sender, e) =>
			{
				var errorMsg = ViewModel.Challenge.ValidateMatchResults();

				if(errorMsg != null)
				{
					errorMsg?.ToToast(ToastNotificationType.Error, "No can do");
					return;
				}

				bool submit = await DisplayAlert("This will end the match", "Are you sure you want to submit these scores?", "Yes", "No");

				if(submit)
				{
					bool success = false;
					using(new HUD("Posting results..."))
					{
						success = await ViewModel.PostMatchResults();
					}

					if(!success)
						return;
					
					await Navigation.PopModalAsync();

					if(OnMatchResultsPosted != null)
						OnMatchResultsPosted();

					var title = App.CurrentAthlete.Id == ViewModel.Challenge.WinningAthlete.Id ? "Victory!" : "Bummer";
					"Results submitted - congrats to {0}!".Fmt(ViewModel.Challenge.WinningAthlete.Name).ToToast(ToastNotificationType.Success, title);
				}
			};
		}
	}

	public partial class MatchResultsFormXaml : BaseContentPage<MatchResultFormViewModel>
	{
	}
}