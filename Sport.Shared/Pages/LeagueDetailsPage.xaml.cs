using System;
using Xamarin.Forms;
using System.Collections.Generic;

namespace Sport.Shared
{
	public partial class LeagueDetailsPage : LeagueDetailsXaml
	{
		const string _leave = "Cowardly Abandon League";
		const string _rules = "League Rules";
		const string _pastChallenges = "Past Challenges";
		double _imageHeight;

		public LeagueDetailsPage(League league)
		{
			league.RefreshChallenges();
			league.RefreshMemberships();

			ViewModel.League = league;

			if(league.Theme == null)
				App.Current.GetTheme(league);

			Title = ViewModel.League?.Name;
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

		Style _actionButtonStyle;

		public Style ActionButtonStyle
		{
			get
			{
				return _actionButtonStyle;
			}
			set
			{
				if(value != _actionButtonStyle)
				{
					_actionButtonStyle = value;
				}
			}
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
				}
			}
		}

		#endregion

		async protected override void Initialize()
		{
			BarBackgroundColor = ViewModel.League.Theme.Light;
			BarTextColor = ViewModel.League.Theme.Dark;

			InitializeComponent();
			scrollView.Scrolled += (sender, e) => Parallax();
			Parallax();

			btnRefresh.Clicked += async(sender, e) =>
			{
				using(new HUD("Refreshing..."))
				{
					await ViewModel.RefreshLeague();	
				}
			};

			ongoingCard.OnClicked = () =>
			{
				PushChallengeDetailsPage(ViewModel.OngoingChallengeViewModel?.Challenge);
			};

			ongoingCard.OnPostResults = async() =>
			{
				var challenge = ViewModel.CurrentMembership.OngoingChallenge;
				if(challenge == null)
					return;
				
				var page = new MatchResultsFormPage(challenge);
				page.AddDoneButton("Cancel");

				page.OnMatchResultsPosted = () =>
				{
					PushChallengeDetailsPage(challenge, true);
					ViewModel.RefreshLeague();
					rankStrip.Membership = ViewModel.CurrentMembership;
				};

				await Navigation.PushModalAsync(page.GetNavigationPage());
			};

			ongoingCard.OnNudge = async() =>
			{
				using(new HUD("Nudging..."))
				{
					await ViewModel.OngoingChallengeViewModel.NudgeAthlete();
				}

				"{0} has been nudged.".Fmt(ViewModel.OngoingChallengeViewModel.Opponent.Alias).ToToast();
			};
				
			ongoingCard.OnAccepted = async() =>
			{
				bool success;
				using(new HUD("Accepting challenge..."))
				{
					success = await ViewModel.OngoingChallengeViewModel.AcceptChallenge();
				}

				if(success)
				{
					await ViewModel.RefreshLeague();
					"Accepted".ToToast();
				}
			};

			ongoingCard.OnDeclined = async() =>
			{
				var decline = await DisplayAlert("Really?", "Are you sure you want to cowardly decline this honorable duel?", "Yes", "No");

				if(!decline)
					return;

				bool success;
				using(new HUD("Declining..."))
				{
					success = await ViewModel.OngoingChallengeViewModel.DeclineChallenge();
				}

				if(success)
				{
					await ViewModel.RefreshLeague();
					"Challenge declined".ToToast();
				}
			};

			if(ViewModel.League != null && ViewModel.League.CreatedByAthleteId != null && ViewModel.League.CreatedByAthlete == null)
			{
				await ViewModel.LoadAthlete();
			}

			rankStrip.Membership = ViewModel.CurrentMembership; //Binding is not working in XAML for some reason
			rankStrip.OnAthleteClicked = async(membership) =>
			{
				var page = new MembershipDetailsPage(membership.Id) {
					BarBackgroundColor = ViewModel.League.Theme.Light,
					BarTextColor = ViewModel.League.Theme.Dark,
				};

				await Navigation.PushAsync(page);
			};

			MessagingCenter.Subscribe<App>(this, "ChallengesUpdated", async(app) =>
			{
				ViewModel.NotifyPropertiesChanged();
			});
		}

		async void PushChallengeDetailsPage(Challenge challenge, bool refresh = false)
		{
			var details = new ChallengeDetailsPage(challenge);
			details.OnAccept = async() =>
			{
				await ViewModel.RefreshLeague();
			};

			details.OnDecline = async() =>
			{
				await ViewModel.RefreshLeague();
			};

			details.OnPostResults = async() =>
			{
				await ViewModel.RefreshLeague();
				rankStrip.Membership = ViewModel.CurrentMembership;
			};

			if(refresh)
				await details.ViewModel.RefreshChallenge();

			await Navigation.PushAsync(details);
		}

		protected override void OnAppearing()
		{
			ViewModel.NotifyPropertiesChanged();
			rankStrip.Membership = ViewModel.CurrentMembership;
			base.OnAppearing();
		}

		protected override async void OnIncomingPayload(App app, NotificationPayload payload)
		{
			string leagueId;
			string winnerId;
			string challengeId;
			if(payload.Payload.TryGetValue("leagueId", out leagueId))
			{
				if(leagueId == ViewModel.League.Id)
				{
					var challenge = ViewModel.CurrentMembership?.OngoingChallenge;
					payload.Payload.TryGetValue("challengeId", out challengeId);

					if(challenge != null && challengeId == challenge.Id && payload.Payload.TryGetValue("winningAthleteId", out winnerId))
					{
						PushChallengeDetailsPage(ViewModel.OngoingChallengeViewModel?.Challenge, true);
					}

					await ViewModel.RefreshLeague();
					Device.BeginInvokeOnMainThread(() =>
					{
						rankStrip.Membership = ViewModel.CurrentMembership;
					});
				}
			}
		}

		void Parallax()
		{
			if(_imageHeight <= 0)
				_imageHeight = photoImage.Height;

			var y = scrollView.ScrollY * .4;
			//var thresh = Device.OS == TargetPlatform.iOS ? 0 : 40;
			if(y < 0)
			{
				//Calculate a scale that equalizes the height vs scroll
				double newHeight = _imageHeight + (scrollView.ScrollY * -1);
				photoImage.Scale = newHeight / _imageHeight;
				photoImage.TranslationY = scrollView.ScrollY / 2;
			}
			else
			{
				photoImage.Scale = 1;
				photoImage.TranslationY = y;
			}
		}

		List<string> GetMoreMenuOptions()
		{
			var list = new List<string>();

			if(ViewModel.CanGetRules)
				list.Add(_rules);

			if(ViewModel.IsMember)
				list.Add(_leave);

			return list;
		}

		async void OnMoreClicked()
		{
			var list = GetMoreMenuOptions();
			var action = await DisplayActionSheet("Additional actions", "Cancel", null, list.ToArray());

			if(action == _leave)
				OnLeaveLeague();

			if(action == _rules)
				OnOpenRules();

			if(action == _pastChallenges)
				DisplayPastChallenges();
		}

		void DisplayPastChallenges()
		{
			"This feature hasn't been implemented".ToToast();
		}

		async void OnRankings()
		{
			if(!ViewModel.League.HasStarted)
			{
				"This league hasn't started".ToToast();
				return;
			}

			var leaderboard = new LeaderboardPage(ViewModel.League);
			await Navigation.PushAsync(leaderboard);
		}

		async void OnOpenRules()
		{
			var page = new BaseContentPage<BaseViewModel> {
				Title = "Rules",
				BarBackgroundColor = BarBackgroundColor,
				BarTextColor = BarTextColor,
				Content = new WebView {
					Source = ViewModel.League.RulesUrl,
				}
			};

			page.AddDoneButton();
			await Navigation.PushModalAsync(page.GetNavigationPage());
		}

		async void OnJoinLeague()
		{
			bool success;
			using(new HUD("Joining..."))
			{
				success = await ViewModel.JoinLeague();
			}

			if(success)
			{
				"Membership approved!".Fmt(ViewModel.League.Name).ToToast(ToastNotificationType.Success);

				if(OnJoinedLeague != null)
				{
					OnJoinedLeague(ViewModel.League);
				}
			}
		}

		async void OnLeaveLeague()
		{
			var accepted = await DisplayAlert("Abandon League?", "Are you sure you want to abandon this league?", "Yes", "No");

			if(accepted)
			{
				var challenge = ViewModel.League.OngoingChallenges.InvolvingAthlete(App.CurrentAthlete.Id);
				if(challenge != null)
				{
					var message = "You have a challenge scheduled with {0}? Still abandon?".Fmt(challenge.Opponent(App.CurrentAthlete.Id).Alias);
					accepted = await DisplayAlert("Existing Challenges", message, "Yes", "No");
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

		void HandleRulesClicked(object sender, EventArgs e)
		{
			OnOpenRules();
		}

		void HandleLeaveClicked(object sender, EventArgs e)
		{
			OnLeaveLeague();
		}

		void HandleRankingsClicked(object sender, EventArgs e)
		{
			OnRankings();
		}

		void HandleJoinClicked(object sender, EventArgs e)
		{
			OnJoinLeague();
		}
	}

	public partial class LeagueDetailsXaml : BaseContentPage<LeagueDetailsViewModel>
	{
	}
}