using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

[assembly: Dependency(typeof(SportChallengeMatchRank.Shared.LeagueLandingViewModel))]
namespace SportChallengeMatchRank.Shared
{
	public class LeagueLandingViewModel : BaseViewModel
	{
		bool _hasLoadedBefore;

		public LeagueLandingViewModel()
		{
			LocalRefresh();
		}

		ObservableCollection<League> _allLeagues = new ObservableCollection<League>();
		public const string AllLeaguesPropertyName = "AllLeagues";

		public ObservableCollection<League> AllLeagues
		{
			get
			{
				return _allLeagues;
			}
			set
			{
				SetProperty(ref _allLeagues, value, AllLeaguesPropertyName);
			}
		}

		public ICommand GetAllLeaguesCommand
		{
			get
			{
				return new Command(async() => await GetAllLeagues(true));
			}
		}

		public void LocalRefresh()
		{
			AllLeagues.Clear();
			DataManager.Instance.Leagues.Values.OrderBy(l => l.Name).ToList().ForEach(AllLeagues.Add);
		}

		async public Task GetAllLeagues(bool forceRefresh = false)
		{
			if(_hasLoadedBefore && !forceRefresh)
				return;
			
			AllLeagues.Clear();
			await RunSafe(AzureService.Instance.GetAllLeagues());
			_hasLoadedBefore = true;
			LocalRefresh();
		}
	}
}