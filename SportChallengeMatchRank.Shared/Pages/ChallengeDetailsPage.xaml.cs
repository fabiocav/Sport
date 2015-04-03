using Xamarin.Forms;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace SportChallengeMatchRank.Shared
{
	public partial class ChallengeDetailsPage : ChallengeDetailsXaml
	{
		public ChallengeDetailsPage(Challenge member = null)
		{
			ViewModel.Challenge = member ?? new Challenge();
			Initialize();
		}

		public Action OnDelete
		{
			get;
			set;
		}

		protected override void Initialize()
		{
			InitializeComponent();
			Title = "Challenge Details";

			btnAccept.Clicked += async(sender, e) =>
			{
				await ViewModel.AcceptChallenge();
			};

			btnRevoke.Clicked += async(sender, e) =>
			{
				var decline = await DisplayAlert("Really?", "Are you sure you want to cowardly revoke this honorable duel?", "Sadly, yes", "No - good point");

				if(!decline)
					return;

				await ViewModel.DeclineChallenge();

				if(OnDelete != null)
					OnDelete();

				await Navigation.PopAsync();
			};
			
			btnDecline.Clicked += async(sender, e) =>
			{
				var decline = await DisplayAlert("Really?", "Are you sure you want to cowardly decline this honorable duel?", "Sadly, yes", "No - good point");

				if(!decline)
					return;
					
				await ViewModel.DeclineChallenge();

				if(OnDelete != null)
					OnDelete();
					
				await Navigation.PopAsync();
			};
		}
	}

	public partial class ChallengeDetailsXaml : BaseContentPage<ChallengeDetailsViewModel>
	{
	}
}