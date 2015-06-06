using System.Threading.Tasks;
using System.Linq;

namespace SportChallengeMatchRank.Shared
{
	public class LeagueViewModel : BaseViewModel
	{
		public LeagueViewModel(League league, Athlete athlete = null)
		{
			League = league;
			Athlete = athlete;
		}

		public League League
		{
			get;
			set;
		}

		public Athlete Athlete
		{
			get;
			set;
		}

		public Membership Membership
		{
			get
			{
				return Athlete?.Memberships.FirstOrDefault(m => m.LeagueId == League.Id);
			}
		}

		public bool IsMember
		{
			get
			{
				return App.CurrentAthlete.Memberships.Any(m => m.LeagueId == League.Id);
			}
		}

		public void LocalRefresh()
		{
		}

		async public Task GetAllMemberships(bool forceRefresh = false)
		{
			if(!forceRefresh)
				return;

			using(new Busy(this))
			{
				LocalRefresh();

				var task = AzureService.Instance.GetAllAthletesForLeague(League);
				await RunSafe(task);
				League.RefreshMemberships();
			}

			IsBusy = false;
		}
	}
}