using System.Threading.Tasks;
using System.Linq;

namespace SportChallengeMatchRank.Shared
{
	public class LeagueViewModel : BaseViewModel
	{
		public LeagueViewModel(League league)
		{
			League = league;	
		}

		public League League
		{
			get;
			set;
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

				var task = AzureService.Instance.GetAllAthletesByLeague(League);
				await RunSafe(task);
				League.RefreshMemberships();
			}

			IsBusy = false;
		}
	}
}