using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Windows.Input;

[assembly: Dependency(typeof(SportChallengeMatchRank.Shared.MembershipsLandingViewModel))]
namespace SportChallengeMatchRank.Shared
{
	public class MembershipsLandingViewModel : BaseViewModel
	{
		Athlete _athlete;
		public const string AthletePropertyName = "Athlete";

		public Athlete Athlete
		{
			get
			{
				return _athlete;
			}
			set
			{
				SetProperty(ref _athlete, value, AthletePropertyName);
			}
		}

		League _league;
		public const string LeaguePropertyName = "League";

		public League League
		{
			get
			{
				return _league;
			}
			set
			{
				SetProperty(ref _league, value, LeaguePropertyName);
			}
		}

		public ICommand GetAllMembershipsByAthleteCommand
		{
			get
			{
				return new Command(async() => await GetAllMembershipsByAthlete(true));
			}
		}

		async public Task GetAllMembershipsByAthlete(bool forceRefresh = false)
		{
			if(!forceRefresh && Athlete.Memberships != null && Athlete.Memberships.Count > 0)
				return;
			
			using(new Busy(this))
			{
				await AzureService.Instance.GetAllLeaguesByAthlete(Athlete);

			}
		}

		public ICommand GetAllMembershipsByLeagueCommand
		{
			get
			{
				return new Command(async() => await GetAllMembershipsByLeague(true));
			}
		}

		async public Task GetAllMembershipsByLeague(bool forceRefresh = false)
		{
			if(!forceRefresh && League.Memberships != null && League.Memberships.Count > 0)
				return;

			using(new Busy(this))
			{
				await AzureService.Instance.GetAllAthletesByLeague(League);

			}
		}
	}
}