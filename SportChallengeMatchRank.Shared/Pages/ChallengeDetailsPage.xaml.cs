using Xamarin.Forms;
using System.Threading.Tasks;
using System;
using System.Linq;

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
				await ViewModel.AcceptChallenge();
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

				await ViewModel.DeclineChallenge();

				if(OnDecline != null)
					OnDecline();

				await Navigation.PopAsync();
			};
			
			btnDecline.Clicked += async(sender, e) =>
			{
				var decline = await DisplayAlert("Really?", "Are you sure you want to cowardly decline this honorable duel?", "Sadly, yes", "No - good point");

				if(!decline)
					return;
					
				await ViewModel.DeclineChallenge();

				if(OnDecline != null)
					OnDecline();
					
				await Navigation.PopAsync();
			};

			await ViewModel.GetMatchResults();
		}
	}

	public partial class ChallengeDetailsXaml : BaseContentPage<ChallengeDetailsViewModel>
	{
	}
}