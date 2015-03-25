using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Windows.Input;
using System.Linq;

[assembly: Dependency(typeof(SportChallengeMatchRank.Shared.AvailableLeaguesViewModel))]
namespace SportChallengeMatchRank.Shared
{
	public class AvailableLeaguesViewModel : BaseViewModel
	{
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

		public ICommand GetAllLeaguesCommand
		{
			get
			{
				return new Command(async() => await GetAvailableLeagues());
			}
		}

		public void LocalRefresh()
		{
			Leagues.Clear();
			DataManager.Instance.Leagues.Where(k => !App.CurrentAthlete.Memberships.Select(m => m.LeagueId).Contains(k.Key)).Select(k => k.Value).ToList().ForEach(Leagues.Add);
		}

		async public Task GetAvailableLeagues()
		{
			Leagues.Clear();
			using(new Busy(this))
			{
				await Task.Delay(1000);
				await AzureService.Instance.GetAllLeagues();
				LocalRefresh();
			}
		}
	}
}