using System.Threading.Tasks;
using Xamarin.Forms;
using System.Windows.Input;

[assembly: Dependency(typeof(SportChallengeMatchRank.Shared.AthleteLeaguesViewModel))]
namespace SportChallengeMatchRank.Shared
{
	public class AthleteLeaguesViewModel : BaseViewModel
	{
		bool _hasLoadedBefore;
		string _athleteId;

		public string AthleteId
		{
			get
			{
				return _athleteId;
			}
			set
			{
				_athleteId = value;
				OnPropertyChanged("Athlete");
				GetLeagues();
			}
		}

		public Athlete Athlete
		{
			get
			{
				return AthleteId == null ? null : DataManager.Instance.Athletes.Get(AthleteId);
			}
		}

		public ICommand GetLeaguesCommand
		{
			get
			{
				return new Command(async() => await GetLeagues(true));
			}
		}

		async public Task GetLeagues(bool forceRefresh = false)
		{
			if(Athlete == null)
				return;

			if(!forceRefresh && _hasLoadedBefore)
			{
				Athlete.RefreshChallenges();
				return;
			}

			if(IsBusy)
				return;

			Athlete.RefreshMemberships();
			await RunSafe(AzureService.Instance.GetAllLeaguesByAthlete(App.CurrentAthlete));
			_hasLoadedBefore = true;
			Athlete.RefreshMemberships();
			OnPropertyChanged("Athlete");
		}
	}
}