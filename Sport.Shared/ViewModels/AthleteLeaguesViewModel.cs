using System.Threading.Tasks;
using Xamarin.Forms;
using System.Windows.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

[assembly: Dependency(typeof(Sport.Shared.AthleteLeaguesViewModel))]
namespace Sport.Shared
{
	public class AthleteLeaguesViewModel : BaseViewModel
	{
		bool _hasLoadedLeaguesBefore;
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

		public ObservableCollection<LeagueViewModel> Leagues
		{
			get;
			set;
		}

		public AthleteLeaguesViewModel()
		{
			Leagues = new ObservableCollection<LeagueViewModel>();
		}

		async public Task GetLeagues(bool forceRefresh = false)
		{
			if(_hasLoadedLeaguesBefore && !forceRefresh)
			{
				Athlete.RefreshMemberships();
				return;
			}

			using(new Busy(this))
			{
				await AthleteViewModel.GetLeagues(forceRefresh);
				LocalRefresh();

				Settings.Instance.LeagueColors.Clear();
				AthleteViewModel.Athlete.Leagues.EnsureLeaguesThemed(true);
			}

			_hasLoadedLeaguesBefore = true;
		}

		public async Task RemoteRefresh()
		{
			await GetLeagues(true);
		}

		public void LocalRefresh()
		{
			if(Athlete == null)
				return;

			if(Leagues == null)
			{
				Athlete.Leagues.OrderBy(l => l.Name).ToList().ForEach(l => Leagues.Add(new LeagueViewModel(l, Athlete)));
			}

			var comparer = new LeagueComparer();
			var toRemove = Leagues.Select(vm => vm.League).Except(Athlete.Leagues, comparer).ToList();
			var toAdd = Athlete.Leagues.Except(Leagues.Select(vm => vm.League), comparer).OrderBy(r => r.Name).ToList();

			var newIds = toAdd.Select(l => l.Id);
			var existing = Leagues.Where(l => !newIds.Contains(l.League.Id)).ToList();

			toRemove.ForEach(l => Leagues.Remove(Leagues.Single(vm => vm.League == l)));
			toAdd.ForEach(l => Leagues.Add(new LeagueViewModel(l, App.CurrentAthlete)));
			Leagues.Sort(new LeagueSortComparer());

			existing.ForEach(l => l.LocalRefresh());

			foreach(var l in Leagues)
				l.IsLast = false;

			var last = Leagues.LastOrDefault();
			if(last != null)
				last.IsLast = true;
			
			if(Leagues.Count == 0)
			{
				Leagues.Add(new LeagueViewModel(new League {
					Name = "You don't belong to any leagues.\n\n\nYou can join leagues by tapping the + button above."
				}));
			}
		}
	}
}