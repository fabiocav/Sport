using Xamarin.Forms;
using System;

namespace Sport.Shared
{
	public partial class ChallengeHistoryPage : ChallengeHistoryPageXaml
	{
		public ChallengeHistoryPage(Membership membership)
		{
			BarBackgroundColor = membership.League.Theme.Light;
			BarTextColor = membership.League.Theme.Dark;

			ViewModel.Membership = membership;
			Initialize();
		}

		protected override void Initialize()
		{
			InitializeComponent();
			Title = "Challenge History";

			list.ItemSelected += async(sender, e) =>
			{
				var vm = e.SelectedItem as ChallengeViewModel;

				if(vm == null)
					return;

				list.SelectedItem = null;
				var details = new ChallengeDetailsPage(vm.Challenge);
				await Navigation.PushAsync(details);
			};
		}

		//		protected override async void OnIncomingPayload(App app, NotificationPayload payload)
		//		{
		//			string leagueId;
		//			if(payload.Payload.TryGetValue("leagueId", out leagueId))
		//			{
		//				if(leagueId == ViewModel.League.Id)
		//				{
		//					await ViewModel.GetLeaderboard(true);
		//				}
		//			}
		//		}
	}

	public partial class ChallengeHistoryPageXaml : BaseContentPage<ChallengeHistoryViewModel>
	{
	}
}