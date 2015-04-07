using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Windows.Input;
using System.Linq;
using System;
using System.Threading;

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
		public const string LeaguesPropertyName = "Leagues";

		public ObservableCollection<League> Leagues
		{
			get
			{
				return _leagues;
			}
			set
			{
				SetProperty(ref _leagues, value, LeaguesPropertyName);
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
			await RunSafe(AzureService.Instance.GetAllLeagues());
			_hasLoadedBefore = true;
			LocalRefresh();

			return;
		}
	}
}