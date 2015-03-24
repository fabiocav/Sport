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
		public AthleteLeaguesViewModel()
		{
			LocalRefresh();
		}

		ObservableCollection<League> _allLeagues = new ObservableCollection<League>();
		public const string AllLeaguesPropertyName = "AllLeagues";

		public ObservableCollection<League> AllLeagues
		{
			get
			{
				return _allLeagues;
			}
			set
			{
				SetProperty(ref _allLeagues, value, AllLeaguesPropertyName);
			}
		}

		public ICommand GetAllLeaguesCommand
		{
			get
			{
				return new Command(async() => await GetLeagues());
			}
		}

		public void LocalRefresh()
		{
			AllLeagues.Clear();
			DataManager.Instance.Leagues.Values.ToList().ForEach(AllLeagues.Add);
		}

		async public Task GetLeagues()
		{
			AllLeagues.Clear();
			using(new Busy(this))
			{
				await Task.Delay(1000);
				await AzureService.Instance.GetAllLeagues();
				LocalRefresh();
			}
		}
	}
}