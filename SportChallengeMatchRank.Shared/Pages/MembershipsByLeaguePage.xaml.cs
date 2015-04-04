using Xamarin.Forms;
using System;

namespace SportChallengeMatchRank.Shared
{
	public partial class MembershipsByLeaguePage : MembershipsByLeagueXaml
	{
		public MembershipsByLeaguePage(League league)
		{
			ViewModel.League = league;
			Initialize();
		}

		protected override void Initialize()
		{
			InitializeComponent();
			Title = "Current Rankings";

			list.ItemSelected += async(sender, e) =>
			{
				if(list.SelectedItem == null)
					return;

				var membership = list.SelectedItem as Membership;
				list.SelectedItem = null;
				await Navigation.PushAsync(new MembershipDetailsPage(membership));
			};
		}


		async protected override void OnLoaded()
		{
			if(ViewModel.League != null)
			{
				await ViewModel.GetAllMembershipsByLeague();
			}

			if(ViewModel.League != null)
				ViewModel.League.RefreshMemberships();
			
			base.OnLoaded();
		}

		//		protected override void OnAppearing()
		//		{
		//			base.OnAppearing();
		//
		//			if(!_dataLoaded)
		//				return;
		//		}
	}

	public partial class MembershipsByLeagueXaml : BaseContentPage<MembershipsLandingViewModel>
	{
	}
}