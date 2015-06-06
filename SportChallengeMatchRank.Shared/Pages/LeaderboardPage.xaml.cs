﻿using Xamarin.Forms;
using System;

namespace SportChallengeMatchRank.Shared
{
	public partial class LeaderboardPage : LeaderboardXaml
	{
		public LeaderboardPage(League league)
		{
			BarBackgroundColor = league.Theme.Light;
			BarTextColor = league.Theme.Dark;

			ViewModel.League = league;
			Initialize();
		}

		protected override void Initialize()
		{
			InitializeComponent();
			Title = "Leaderboard";

			list.ItemSelected += async(sender, e) =>
			{
				if(list.SelectedItem == null)
					return;

				var membership = list.SelectedItem as Membership;
				list.SelectedItem = null;
				var page = new MembershipDetailsPage(membership.Id) {
					BarBackgroundColor = ViewModel.League.Theme.Light,
					BarTextColor = ViewModel.League.Theme.Dark,
				};
					
				await Navigation.PushAsync(page);
			};

			if(ViewModel.League != null)
				ViewModel.LocalRefresh();
		}

		protected override void OnAppearing()
		{
			if(ViewModel.League != null)
				ViewModel.LocalRefresh();

			base.OnAppearing();
		}

		void OnChallengeClicked(object sender, EventArgs e)
		{
			
		}

		protected override async void OnIncomingPayload(App app, NotificationPayload payload)
		{
			string leagueId = null;
			if(payload.Payload.TryGetValue("leagueId", out leagueId))
			{
				if(ViewModel.League != null && leagueId == ViewModel.League.Id)
				{
					await ViewModel.GetLeaderboard(true);
				}
			}
		}
	}

	public partial class LeaderboardXaml : BaseContentPage<LeaderboardViewModel>
	{
	}
}