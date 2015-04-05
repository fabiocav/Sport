using System.Threading.Tasks;
using Xamarin.Forms;
using System.Windows.Input;

[assembly: Dependency(typeof(SportChallengeMatchRank.Shared.AthleteChallengesViewModel))]
namespace SportChallengeMatchRank.Shared
{
	public class AthleteChallengesViewModel : BaseViewModel
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
				GetChallenges();
			}
		}

		public Athlete Athlete
		{
			get
			{
				return AthleteId == null ? null : DataManager.Instance.Athletes.Get(AthleteId);
			}
		}

		public ICommand GetChallengesCommand
		{
			get
			{
				return new Command(async() => await GetChallenges(true));
			}
		}

		async public Task GetChallenges(bool forceRefresh = false)
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

			Athlete.RefreshChallenges();
			using(new Busy(this))
			{
				//Load the opponents
				await AzureService.Instance.GetAllChallengesByAthlete(Athlete);

				foreach(var c in DataManager.Instance.Challenges.Values)
				{
					if(c.ChallengeeAthlete == null)
					{
						await AzureService.Instance.GetAthleteById(c.ChallengeeAthleteId);
					}

					if(c.ChallengerAthlete == null)
					{
						await AzureService.Instance.GetAthleteById(c.ChallengerAthleteId);
					}
				}

				_hasLoadedBefore = true;
				Athlete.RefreshChallenges();
				OnPropertyChanged("Athlete");
			}
		}
	}
}