using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Windows.Input;

[assembly: Dependency(typeof(SportRankerMatchOn.Shared.LeagueLandingViewModel))]
namespace SportRankerMatchOn.Shared
{
	public class LeagueLandingViewModel : BaseViewModel
	{
		public LeagueLandingViewModel()
		{
			AllLeagues = new ObservableCollection<League>();
		}

		public ObservableCollection<League> AllLeagues
		{
			get;
			set;
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

