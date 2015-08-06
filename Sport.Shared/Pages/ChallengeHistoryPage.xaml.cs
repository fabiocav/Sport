using Xamarin.Forms;
using System;
using System.Collections.Generic;

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

		protected override void TrackPage(Dictionary<string, string> metadata)
		{
			if(ViewModel?.Membership != null)
				metadata.Add("membershipId", ViewModel.Membership.Id);

			base.TrackPage(metadata);
		}
	}

	public partial class ChallengeHistoryPageXaml : BaseContentPage<ChallengeHistoryViewModel>
	{
	}
}