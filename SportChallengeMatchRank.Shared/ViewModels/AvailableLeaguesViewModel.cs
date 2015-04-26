using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

[assembly: Dependency(typeof(SportChallengeMatchRank.Shared.AvailableLeaguesViewModel))]
namespace SportChallengeMatchRank.Shared
{
	public class AvailableLeaguesViewModel : BaseViewModel
	{
		bool _hasLoadedBefore;

		public AvailableLeaguesViewModel()
		{
			LocalRefresh();
		}

		ObservableCollection<League> _leagues = new ObservableCollection<League>();

		public ObservableCollection<League> Leagues
		{
			get
			{
				return _leagues;
			}
			set
			{
				SetPropertyChanged(ref _leagues, value);
			}
		}

		public ICommand GetAvailableLeaguesCommand
		{
			get
			{
				return new Command(async() => await GetAvailableLeagues(true));
			}
		}

		public void LocalRefresh()
		{
			if(App.CurrentAthlete == null)
				return;

			Leagues.Clear();
			DataManager.Instance.Leagues.Where(k => !App.CurrentAthlete.Memberships.Select(m => m.LeagueId).Contains(k.Key)).Select(k => k.Value).ToList().ForEach(Leagues.Add);

			if(Leagues.Count == 0)
			{
				Leagues.Add(new League {
					Name = "There are no joinable leagues",
				});
			}
		}

		async public Task GetAvailableLeagues(bool forceRefresh = false)
		{
			if(App.CurrentAthlete == null)
				return;

			if(!forceRefresh && _hasLoadedBefore)
			{
				LocalRefresh();
				return;
			}

			Leagues.Clear();
			await RunSafe(InternetService.Instance.GetAllLeagues());
			_hasLoadedBefore = true;
			LocalRefresh();
		}
	}
}