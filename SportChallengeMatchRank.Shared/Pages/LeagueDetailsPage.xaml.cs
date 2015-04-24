using Xamarin.Forms;
using System.Threading.Tasks;
using System.Linq;
using System;
using Toasts.Forms.Plugin.Abstractions;

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

		async protected override void Initialize()
		{
			Title = "League Details";
			InitializeComponent();

			btnMemberStatus.Clicked += async(sender, e) =>
			{
				if(!ViewModel.League.HasStarted)
				{
					"This league hasn't started yet so let's everyone just calm down and hold your horses, mkay?".ToToast(ToastNotificationType.Warning, "No can do");
					return;
				}

				if(_membershipsPage == null)
					_membershipsPage = new MembershipsByLeaguePage(ViewModel.League);

				await Navigation.PushAsync(_membershipsPage);	
			};

			btnJoinLeague.Clicked += async(sender, e) =>
			{
				var accepted = await DisplayAlert("Join League?", "Are you totes sure you want to join this league?", "Yesh", "No");

				if(accepted)
				{
					var success = await ViewModel.JoinLeague();

					if(success)
					{
						"You are now a member of {0}".Fmt(ViewModel.League.Name).ToToast(ToastNotificationType.Success, "Behold!");

						if(OnJoinedLeague != null)
						{
							OnJoinedLeague(ViewModel.League);
						}
					}
				}
			};

			btnLeaveLeague.Clicked += async(sender, e) =>
			{
				var accepted = await DisplayAlert("Abandon League?", "Are you totes sure you want to abandon this league like a heaping pile of slime?", "Yeps", "Nah");

				if(accepted)
				{
					if(App.CurrentAthlete.AllChallenges.Any(c => c.LeagueId == ViewModel.League.Id))
					{
						accepted = await DisplayAlert("Existing Challenges?", "You have ongoing challenges - still quit?", "Yeps", "Nah");
					}

					if(accepted)
					{
						await ViewModel.LeaveLeague();
						"Can't leave this league because you have been already been ejected, so there :P".ToToast(ToastNotificationType.Info, "No can do");
						if(OnAbandondedLeague != null)
						{
							OnAbandondedLeague(ViewModel.League);
						}
					}
				}
			};

			btnEditLeague.Clicked += async(sender, e) =>
			{
				var detailsPage = new LeagueEditPage(ViewModel.League);
				detailsPage.OnUpdate = () =>
				{
					ViewModel.SetPropertyChanged("League");
				};

				await Navigation.PushModalAsync(new NavigationPage(detailsPage));
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