using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Windows.Input;

[assembly: Dependency(typeof(SportChallengeMatchRank.Shared.LeagueLandingViewModel))]
namespace SportChallengeMatchRank.Shared
{
	public class LeagueLandingViewModel : BaseViewModel
	{
		public LeagueLandingViewModel()
		{
			AllLeagues = new ObservableCollection<League>();
		}

		ObservableCollection<League> _allLeagues;
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

		async public Task GetAllLeagues(bool forceRefresh = false)
		{
			if(!forceRefresh && AllLeagues.Count > 0)
				return;
			
			using(new Busy(this))
			{
				AllLeagues.Clear();
				var list = await AzureService.Instance.GetAllLeagues();
				list.ForEach(AllLeagues.Add);
			}
		}
	}
}

