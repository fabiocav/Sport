using Xamarin.Forms;
using System.Threading.Tasks;
using System.Linq;
using System;

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
					await DisplayAlert("Still Recruitin', y'all", "This league hasn't started yet so let's everyone just calm down and hold your horses, mkay?", "kay");
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
					await ViewModel.JoinLeague();
					if(OnJoinedLeague != null)
					{
						OnJoinedLeague(ViewModel.League);
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
						if(OnAbandondedLeague != null)
						{
							OnAbandondedLeague(ViewModel.League);
						}
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