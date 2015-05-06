using System.Threading.Tasks;
using Xamarin.Forms;
using System.Windows.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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
				SetPropertyChanged("Athlete");
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

		public ObservableCollection<League> Leagues
		{
			get;
			set;
		}

		public AthleteLeaguesViewModel()
		{
			Leagues = new ObservableCollection<League>();	
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

			using(new Busy(this))
			{
				Athlete.RefreshMemberships();
				await RunSafe(AzureService.Instance.GetAllLeaguesByAthlete(App.CurrentAthlete));
				_hasLoadedBefore = true;

				LocalRefresh();
				SetPropertyChanged("Athlete");
			}
		}

		public void LocalRefresh()
		{
			Leagues.Clear();
			Athlete.RefreshMemberships();
			Athlete.Leagues.ForEach(Leagues.Add);

			if(Leagues.Count == 0)
			{
				Leagues.Add(new League {
					Name = "You don't belong to any leagues yet.",
				});
			}
		}
	}
}