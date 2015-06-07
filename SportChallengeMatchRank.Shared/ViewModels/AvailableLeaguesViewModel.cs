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

		ObservableCollection<LeagueViewModel> _leagues = new ObservableCollection<LeagueViewModel>();

		public ObservableCollection<LeagueViewModel> Leagues
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

			var comparer = new LeagueComparer();
			var toJoin = DataManager.Instance.Leagues.Where(k => !App.CurrentAthlete.Memberships.Select(m => m.LeagueId).Contains(k.Key))
				.Select(k => k.Value).ToList();

			var toRemove = Leagues.Select(vm => vm.League).Except(toJoin, comparer).ToList();
			var toAdd = toJoin.Except(Leagues.Select(vm => vm.League), comparer).OrderBy(r => r.Name).ToList();

			toRemove.ForEach(l => Leagues.Remove(Leagues.Single(vm => vm.League == l)));
			toAdd.ForEach(l => Leagues.Add(new LeagueViewModel(l, App.CurrentAthlete)));
			Leagues.Sort(new LeagueSortComparer());

			foreach(var l in Leagues)
			{
				l.IsLast = false;
				App.Current.GetTheme(l.League);
			}

			var last = Leagues.LastOrDefault();
			if(last != null)
				last.IsLast = true;

			if(Leagues.Count == 0)
			{
				Leagues.Add(new LeagueViewModel(new League {
					Name = "There are no more available leagues to join"
				}));
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

			using(new Busy(this))
			{
				await RunSafe(AzureService.Instance.GetAvailableLeagues(App.CurrentAthlete));
				_hasLoadedBefore = true;
				LocalRefresh();
			}
		}
	}
}