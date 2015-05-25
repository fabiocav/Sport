﻿using Xamarin.Forms;
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
				await Navigation.PushAsync(new MembershipDetailsPage(membership.Id));
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

		protected override async void OnIncomingPayload(App app, NotificationPayload payload)
		{
			string leagueId = null;
			if(payload.Payload.TryGetValue("leagueId", out leagueId))
			{
				if(ViewModel.League != null && leagueId == ViewModel.League.Id)
				{
					await ViewModel.GetAllMembershipsByLeague(true);
				}
			}
		}

		public void OnChallengeClicked(object sender, EventArgs e)
		{
			var mi = ((MenuItem)sender);
		}
	}

	public partial class MembershipsByLeagueXaml : BaseContentPage<MembershipsByLeagueViewModel>
	{
	}
}