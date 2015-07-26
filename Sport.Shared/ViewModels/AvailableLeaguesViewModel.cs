using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using System.Collections.Generic;

[assembly: Dependency(typeof(Sport.Shared.AvailableLeaguesViewModel))]
namespace Sport.Shared
{
	public class AvailableLeaguesViewModel : BaseViewModel
	{
		bool _hasLoadedBefore;
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
				Leagues?.Clear();
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
			var preSort = new List<LeagueViewModel>();
			toAdd.ForEach(l => preSort.Add(new LeagueViewModel(l, App.CurrentAthlete)));
			preSort.Sort(new LeagueSortComparer());

			var last = preSort.LastOrDefault();
			preSort.ForEach(l => l.IsLast = l == last);
			preSort.ForEach(Leagues.Add);

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
				var task = AzureService.Instance.GetAvailableLeagues(App.CurrentAthlete);
				await RunSafe(task);

				if(task.IsCompleted && !task.IsFaulted)
				{
					task.Result.EnsureLeaguesThemed();

					_hasLoadedBefore = true;
					LocalRefresh();
				}
			}
		}
	}
}