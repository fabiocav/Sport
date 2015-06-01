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
		bool _hasLoadedLeaguesBefore;
		bool _hasLoadedChallengesBefore;
		string _athleteId;

		public AthleteViewModel AthleteViewModel
		{
			get;
			set;
		}

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
				AthleteViewModel = new AthleteViewModel(Athlete);
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
				return new Command(async() => await RemoteRefresh());
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
			if(_hasLoadedLeaguesBefore && !forceRefresh)
			{
				Athlete.RefreshChallenges();
				return;
			}

			using(new Busy(this))
			{
				await AthleteViewModel.GetLeagues(forceRefresh |= !_hasLoadedChallengesBefore);
				LocalRefresh();
			}

			_hasLoadedLeaguesBefore = true;
		}

		async public Task GetChallenges(bool forceRefresh = false)
		{
			if(_hasLoadedChallengesBefore && !forceRefresh)
			{
				Athlete.RefreshChallenges();
				return;
			}
			
			await AthleteViewModel.GetChallenges(forceRefresh |= !_hasLoadedChallengesBefore);
			Athlete.RefreshChallenges();
			_hasLoadedChallengesBefore = true;
		}

		public async Task RemoteRefresh()
		{
			await GetLeagues(true);
			await GetChallenges(true);
		}

		public void LocalRefresh()
		{
			var comparer = new LeagueComparer();
			var toRemove = Leagues.Except(Athlete.Leagues, comparer).ToList();
			var toAdd = Athlete.Leagues.Except(Leagues, comparer).OrderBy(r => r.Name).ToList();

			toRemove.ForEach(l => Leagues.Remove(l));
			toAdd.ForEach(Leagues.Add);

			if(Leagues.Count == 0)
			{
				Leagues.Add(new League {
					Name = "Join a league",
				});
			}
		}

		public class LeagueComparer : IEqualityComparer<League>
		{
			public bool Equals(League x, League y)
			{
				return x?.Id == y?.Id && x.UpdatedAt == y.UpdatedAt;
			}

			public int GetHashCode(League obj)
			{
				return obj.Id != null ? obj.Id.GetHashCode() : base.GetHashCode();
			}
		}
	}
}