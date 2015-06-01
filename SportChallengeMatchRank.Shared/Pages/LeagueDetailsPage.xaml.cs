using System.Linq;
using System;
using Xamarin.Forms;
using System.Collections.Generic;

namespace SportChallengeMatchRank.Shared
{
	public partial class LeagueDetailsPage : LeagueDetailsXaml
	{
		MembershipsByLeaguePage _membershipsPage;

		public LeagueDetailsPage(League league)
		{
			ViewModel.League = league;
			Initialize();
		}

		public Action<League> OnJoinedLeague
		{
			get;
			set;
		}

		public Action<League> OnAbandondedLeague
		{
			get;
			set;
		}

		Style _buttonStyle;

		public Style ButtonStyle
		{
			get
			{
				return _buttonStyle;
			}
			set
			{
				if(value != _buttonStyle)
				{
					_buttonStyle = value;
					btnRankings.Style = btnJoin.Style = _buttonStyle;
				}
			}
		}

		async protected override void Initialize()
		{
			Title = "League Details";
			InitializeComponent();

			var moreButton = new ToolbarItem("More", "ic_more_vert_white", () =>
			{
				OnMoreClicked();
			});

			if(GetMoreMenuOptions().Count > 0)
				ToolbarItems.Add(moreButton);

			btnRankings.Clicked += async(sender, e) =>
			{
				if(!ViewModel.League.HasStarted)
				{
					"This league hasn't started".ToToast(ToastNotificationType.Info);
					return;
				}

				if(_membershipsPage == null)
					_membershipsPage = new MembershipsByLeaguePage(ViewModel.League);

				await Navigation.PushAsync(_membershipsPage);	
			};

			btnJoin.Clicked += async(sender, e) =>
			{
				var accepted = await DisplayAlert("Join League?", "Are you sure you want to join this league?", "Yes", "No");

				if(accepted)
				{

					bool success;
					using(new HUD("Joining..."))
					{
						success = await ViewModel.JoinLeague();
					}

					if(success)
					{
						"Congrats!".Fmt(ViewModel.League.Name).ToToast(ToastNotificationType.Success);

						if(OnJoinedLeague != null)
						{
							OnJoinedLeague(ViewModel.League);
						}
					}
				}
			};

			if(ViewModel.League != null && ViewModel.League.CreatedByAthleteId != null && ViewModel.League.CreatedByAthlete == null)
			{
				//using(new HUD("Getting info..."))
				{
					await ViewModel.LoadAthlete();
				}
			}

			using(new Busy(ViewModel))
			{
				ViewModel.RefreshLeague();
			}
		}

		protected override async void OnIncomingPayload(App app, NotificationPayload payload)
		{
			string leagueId;
			if(payload.Payload.TryGetValue("leagueId", out leagueId))
			{
				if(leagueId == ViewModel.League.Id)
				{
					await ViewModel.RefreshLeague();
				}
			}
		}

		const string _leave = "Cowardly Abandon League";
		const string _rules = "League Rules";
		const string _pastChallenges = "Past Challenges";

		List<string> GetMoreMenuOptions()
		{
			var list = new List<string>();

			if(ViewModel.CanGetRules)
				list.Add(_rules);

			if(ViewModel.IsMember && App.CurrentAthlete.AllChallenges.Any(c => c.LeagueId == ViewModel.League.Id && c.IsCompleted))
				list.Add(_pastChallenges);

			if(ViewModel.IsMember)
				list.Add(_leave);

			return list;
		}

		async void OnMoreClicked()
		{
			var list = GetMoreMenuOptions();
			var action = await DisplayActionSheet("Additional actions", "Cancel", null, list.ToArray());

			if(action == _leave)
				LeaveLeague();

			if(action == _rules)
				OpenRules();

			if(action == _pastChallenges)
				DisplayPastChallenges();
		}

		async void DisplayPastChallenges()
		{
		}

		async void OpenRules()
		{
			var page = new ContentPage {
				Title = "Rules",
				Content = new WebView {
					Source = ViewModel.League.RulesUrl,
				}
			};

			var nav = new NavigationPage(page) {
				BarTextColor = Color.White,
				BarBackgroundColor = (Color)App.Current.Resources["greenPrimary"],
			};

			var cancel = new ToolbarItem("Cancel", null, () =>
			{
				Navigation.PopModalAsync();
			});

			nav.ToolbarItems.Add(cancel);
			nav.Title = "Rules";
			await Navigation.PushModalAsync(nav);
		}

		async void LeaveLeague()
		{
			var accepted = await DisplayAlert("Abandon League?", "Are you sure you want to abandon this league?", "Yes", "No");

			if(accepted)
			{
				if(App.CurrentAthlete.AllChallenges.Any(c => c.LeagueId == ViewModel.League.Id))
				{
					accepted = await DisplayAlert("Existing Challenges?", "You have ongoing challenges - still abandon?", "Yes", "No");
				}

				if(accepted)
				{
					using(new HUD("Abandoning..."))
					{
						await ViewModel.LeaveLeague();
					}

					"Sorry to see you go".ToToast(ToastNotificationType.Info);
					if(OnAbandondedLeague != null)
					{
						OnAbandondedLeague(ViewModel.League);
					}
				}
			}
		}
	}

	public partial class LeagueDetailsXaml : BaseContentPage<LeagueDetailsViewModel>
	{
	}
}