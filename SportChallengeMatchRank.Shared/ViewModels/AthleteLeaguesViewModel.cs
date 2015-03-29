using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Windows.Input;
using System.Linq;

[assembly: Dependency(typeof(SportChallengeMatchRank.Shared.AthleteLeaguesViewModel))]
namespace SportChallengeMatchRank.Shared
{
	public class AthleteLeaguesViewModel : BaseViewModel
	{
		bool _hasLoadedBefore;

		public AthleteLeaguesViewModel()
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

		public ICommand GetLeaguesCommand
		{
			get
			{
				return new Command(async() => await GetLeagues(true));
			}
		}

		public void LocalRefresh()
		{
			Leagues.Clear();
			App.CurrentAthlete.Memberships.Select(m => m.League).ToList().ForEach(Leagues.Add);
		}

		async public Task GetLeagues(bool forceRefresh = false)
		{
			if(!forceRefresh && _hasLoadedBefore)
			{
				LocalRefresh();
				return;
			}

			Leagues.Clear();
			using(new Busy(this))
			{
				await AzureService.Instance.GetAllLeaguesByAthlete(App.CurrentAthlete);
				_hasLoadedBefore = true;
				LocalRefresh();
			}
		}
	}
}