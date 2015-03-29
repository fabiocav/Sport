using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Windows.Input;
using System.Linq;

[assembly: Dependency(typeof(SportChallengeMatchRank.Shared.AthleteLandingViewModel))]
namespace SportChallengeMatchRank.Shared
{
	public class AthleteLandingViewModel : BaseViewModel
	{
		bool _hasLoadedBefore;

		public AthleteLandingViewModel()
		{
			AllAthletes = new ObservableCollection<Athlete>();
		}

		public ObservableCollection<Athlete> AllAthletes
		{
			get;
			set;
		}

		public ICommand GetAllAthletesCommand
		{
			get
			{
				return new Command(async() => await GetAllAthletes(true));
			}
		}

		public void LocalRefresh()
		{
			AllAthletes.Clear();
			DataManager.Instance.Athletes.Values.OrderBy(a => a.Name).ToList().ForEach(AllAthletes.Add);
		}

		async public Task GetAllAthletes(bool forceRefresh = false)
		{
			if(_hasLoadedBefore && !forceRefresh)
				return;
			
			AllAthletes.Clear();
			using(new Busy(this))
			{
				await Task.Delay(1000);
				await AzureService.Instance.GetAllAthletes();
				_hasLoadedBefore = true;
				LocalRefresh();
			}
		}
	}
}