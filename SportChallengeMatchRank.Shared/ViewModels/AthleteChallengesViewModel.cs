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
				SetPropertyChanged("Athlete");
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
			{
				return;
			}

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

			ChallengeGroups.Clear();

			//Load the opponents
			var task = InternetService.Instance.GetAllChallengesByAthlete(Athlete);
			await RunSafe(task);

			if(task.IsFaulted)
				return;

			foreach(var c in DataManager.Instance.Challenges.Values)
			{
				if(c.ChallengeeAthlete == null)
				{
					await RunSafe(InternetService.Instance.GetAthleteById(c.ChallengeeAthleteId));
				}

				if(c.ChallengerAthlete == null)
				{
					await RunSafe(InternetService.Instance.GetAthleteById(c.ChallengerAthleteId));
				}
			}

			_hasLoadedBefore = true;
			LocalRefresh();
		}

		public void LocalRefresh()
		{
			ChallengeGroups.Clear();
			UpcomingChallenges.Clear();
			HistoricalChallenges.Clear();

			Athlete.RefreshChallenges();
			SetPropertyChanged("Athlete");

			Athlete.AllChallenges.Where(c => c.IsCompleted).ToList().ForEach(HistoricalChallenges.Add);
			Athlete.AllChallenges.Where(c => !c.IsCompleted).ToList().ForEach(UpcomingChallenges.Add);

			if(UpcomingChallenges.Count > 0)
				ChallengeGroups.Add(UpcomingChallenges);

			if(HistoricalChallenges.Count > 0)
				ChallengeGroups.Add(HistoricalChallenges);
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