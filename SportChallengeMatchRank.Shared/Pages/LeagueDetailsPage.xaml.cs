﻿using Xamarin.Forms;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace SportChallengeMatchRank.Shared
{
	public partial class LeagueDetailsPage : LeagueDetailsXaml
	{
		public LeagueDetailsPage(League league)
		{
			ViewModel.League = league;
			Initialize();
		}

		async protected override void Initialize()
		{
			InitializeComponent();
			Title = "League Details";

			btnMemberStatus.Clicked += async(sender, e) =>
			{
				if(!ViewModel.League.HasStarted)
				{
					await DisplayAlert("Still Recruiting, y'all", "This league hasn't started yet so let's everyone just calm down and hold your horses, mkay?", "kay");
					return;
				}

				await Navigation.PushAsync(new MembershipsByLeaguePage(ViewModel.League));	
			};

			btnJoinLeague.Clicked += async(sender, e) =>
			{
				var accepted = await DisplayAlert("Join League?", "Are you totes sure you want to join this league?", "Yesh", "No");

				if(accepted)
				{
					await ViewModel.JoinLeague();
				}
			};

			btnLeaveLeague.Clicked += async(sender, e) =>
			{
				var accepted = await DisplayAlert("Abandon League?", "Are you totes sure you want to abandon this league like a heaping pile of slime?", "Yeps", "Nah");

				if(accepted)
				{
					if(App.CurrentAthlete.AllChallenges.Count > 0)
					{
						accepted = await DisplayAlert("Existing Challenges?", "You have ongoing challenges - still quit?", "Yeps", "Nah");
					}

					if(accepted)
					{
						await ViewModel.LeaveLeague();
					}
				}
			};

			if(ViewModel.League != null && ViewModel.League.CreatedByAthleteId != null && ViewModel.League.CreatedByAthlete == null)
			{
				await ViewModel.LoadAthlete();
			}
		}
	}

	public partial class LeagueDetailsXaml : BaseContentPage<LeagueDetailsViewModel>
	{
	}
}