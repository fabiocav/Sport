using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

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

		public ObservableCollection<ChallengeCollection> ChallengeGroups
		{
			get;
			set;
		}

		public ChallengeCollection HistoricalChallenges
		{
			get;
			set;
		}

		public ChallengeCollection UpcomingChallenges
		{
			get;
			set;
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

		public AthleteChallengesViewModel()
		{
			UpcomingChallenges = new ChallengeCollection {
				Title = "Ongoing Challenges"
			};

			HistoricalChallenges = new ChallengeCollection {
				Title = "Historical Challenges"
			};

			ChallengeGroups = new ObservableCollection<ChallengeCollection>();
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
			UpcomingChallenges.Clear();
			HistoricalChallenges.Clear();

			using(new Busy(this))
			{
				ChallengeGroups.Clear();

				//Load the opponents
				var task = AzureService.Instance.GetAllChallengesByAthlete(Athlete);
				await RunSafe(task);

				foreach(var c in DataManager.Instance.Challenges.Values)
				{
					if(c.ChallengeeAthlete == null)
					{
						var getAthlete = AzureService.Instance.GetAthleteById(c.ChallengeeAthleteId);
						await RunSafe(getAthlete);
					}

					if(c.ChallengerAthlete == null)
					{
						var getAthlete = AzureService.Instance.GetAthleteById(c.ChallengerAthleteId);
						await RunSafe(getAthlete);
					}
				}

				_hasLoadedBefore = true;
				Athlete.RefreshChallenges();
				OnPropertyChanged("Athlete");

				Athlete.AllChallenges.Where(c => c.IsCompleted).ToList().ForEach(HistoricalChallenges.Add);
				Athlete.AllChallenges.Where(c => !c.IsCompleted).ToList().ForEach(UpcomingChallenges.Add);

				if(UpcomingChallenges.Count > 0)
					ChallengeGroups.Add(UpcomingChallenges);

				if(HistoricalChallenges.Count > 0)
					ChallengeGroups.Add(HistoricalChallenges);
			}					
		}
	}

	public class ChallengeCollection : ObservableCollection<Challenge>
	{
		public string Title
		{
			get;
			set;
		}
	}
}