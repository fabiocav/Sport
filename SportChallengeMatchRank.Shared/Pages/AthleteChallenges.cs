using Xamarin.Forms;

namespace SportChallengeMatchRank.Shared
{
	public partial class AthleteChallengesPage : AthleteChallengesXaml
	{
		async protected override void Initialize()
		{
			InitializeComponent();
			Title = "Challenges";

			list.ItemSelected += async(sender, e) =>
			{
				if(list.SelectedItem == null)
					return;

				var challenge = list.SelectedItem as Challenge;
				list.SelectedItem = null;
				await Navigation.PushAsync(new ChallengeDetailsPage(challenge));
			};

			await ViewModel.GetChallenges();
		}
	}

	public partial class AthleteChallengesXaml : BaseContentPage<AthleteChallengesViewModel>
	{
	}
}