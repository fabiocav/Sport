using System.Threading.Tasks;
using Xamarin.Forms;
using System.Windows.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System;

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
				DataManager.Instance.Leagues.Values.ToList().EnsureLeaguesThemed(true);
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
				Leagues = new ObservableCollection<LeagueViewModel>();

			var comparer = new LeagueIdComparer();
			var toRemove = Leagues.Select(vm => vm.League).Except(Athlete.Leagues, comparer).ToList();
			var toAdd = Athlete.Leagues.Except(Leagues.Select(vm => vm.League), comparer).OrderBy(r => r.Name).Select(l => new LeagueViewModel(l, App.CurrentAthlete)).ToList();
			toRemove.ForEach(l => Leagues.Remove(Leagues.Single(vm => vm.League == l)));

			var eqComparer = new LeagueComparer();
			var changed = Athlete.Leagues.Except(Leagues.Select(vm => vm.League), eqComparer).ToList();

			var compare = new LeagueSortComparer();
			foreach(var lv in toAdd)
			{
				int index = 0;
				foreach(var l in Leagues.ToList())
				{
					if(compare.Compare(lv, l) < 0)
						break;

					index++;
				}
				Leagues.Insert(index, lv);
			}

			var last = Leagues.LastOrDefault();
			foreach(var l in Leagues)
				l.IsLast = l == last;

			//Not updating for some reason
			foreach(var league in changed)
			{
				var vm = Leagues.SingleOrDefault(v => v.League.Id == league.Id);
				if(vm != null)
				{
					vm.League = league;
					vm.LocalRefresh();
				}
			}

			if(Leagues.Count == 0)
			{
				Leagues.Add(new LeagueViewModel(null) {
					EmptyMessage = "You don't belong to any leagues... yet.\n\n\n\nYou can and will join leagues by tapping the + button above."
				});
			}
		}
	}
}