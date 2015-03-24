using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Windows.Input;
using System.Linq;

[assembly: Dependency(typeof(SportChallengeMatchRank.Shared.LeagueLandingViewModel))]
namespace SportChallengeMatchRank.Shared
{
	public class LeagueLandingViewModel : BaseViewModel
	{
		public LeagueLandingViewModel()
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
				return new Command(async() => await GetAllLeagues(true));
			}
		}

		public void LocalRefresh()
		{
			AllLeagues.Clear();
			DataManager.Instance.Leagues.Values.ToList().ForEach(AllLeagues.Add);
		}

		async public Task GetAllLeagues(bool forceRefresh = false)
		{
			if(!forceRefresh && AllLeagues.Count > 0)
				return;
			
			using(new Busy(this))
			{
				await AzureService.Instance.GetAllLeagues();
				LocalRefresh();
			}
		}
	}
}