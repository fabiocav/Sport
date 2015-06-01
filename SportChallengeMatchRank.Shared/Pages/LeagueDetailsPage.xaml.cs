using System.Linq;
using System;
using Xamarin.Forms;
using System.Collections.Generic;

namespace SportChallengeMatchRank.Shared
{
	public partial class LeagueDetailsPage : LeagueDetailsXaml
	{
		public LeagueDetailsPage(League league)
		{
			ViewModel.League = league;
			Initialize();
		}

		#region Properties

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
					btnJoin.Style = _buttonStyle;
				}
			}
		}

		#endregion

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

			btnJoin.Clicked += async(sender, e) =>
			{
				var accepted = await DisplayAlert("Join League?", "Are you sure you want to join this league?", "Yes", "No");

				if(accepted)
				{

					bool success;
					using(new HUD("Applying for membership..."))
					{
						success = await ViewModel.JoinLeague();
					}

					if(success)
					{
						"Membership accepted!".Fmt(ViewModel.League.Name).ToToast(ToastNotificationType.Success);

						if(OnJoinedLeague != null)
						{
							OnJoinedLeague(ViewModel.League);
						}
					}
				}
			};

			cardView.OnClicked = async() =>
			{
				var details = new ChallengeDetailsPage(ViewModel.OngoingChallenge);
				details.OnAccept = () =>
				{
					ViewModel.NotifyPropertiesChanged();
				};

				details.OnDecline = () =>
				{
					ViewModel.NotifyPropertiesChanged();
				};

				details.OnPostResults = () =>
				{
					ViewModel.NotifyPropertiesChanged();
				};

				await Navigation.PushAsync(details);
			};

			cardView.OnAccepted = async() =>
			{
				bool success;
				using(new HUD("Accepting challenge..."))
				{
					success = await ViewModel.ChallengeViewModel.AcceptChallenge();
				}

				if(success)
				{
					ViewModel.NotifyPropertiesChanged();
					"Accepted".ToToast();
				}
			};

			cardView.OnDeclined = async() =>
			{
				var decline = await DisplayAlert("Really?", "Are you sure you want to cowardly decline this honorable duel?", "Yes", "No");

				if(!decline)
					return;

				bool success;
				using(new HUD("Declining..."))
				{
					success = await ViewModel.ChallengeViewModel.DeclineChallenge();
				}

				if(success)
				{
					ViewModel.NotifyPropertiesChanged();
					"Unbelievable!".ToToast();
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
				await ViewModel.RefreshLeague();
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
		const string _rankings = "Rankings";

		List<string> GetMoreMenuOptions()
		{
			var list = new List<string>();
			list.Add(_rankings);

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

			if(action == _rankings)
				OnRankingsClicked();
		}

		void DisplayPastChallenges()
		{
			"This feature hasn't been implemented".ToToast();
		}

		async void OnRankingsClicked()
		{
			if(!ViewModel.League.HasStarted)
			{
				"This league hasn't started".ToToast();
				return;
			}

			var leaderboard = new LeaderboardPage(ViewModel.League);
			await Navigation.PushAsync(leaderboard);	
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

					"Sorry to see you go".ToToast();
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