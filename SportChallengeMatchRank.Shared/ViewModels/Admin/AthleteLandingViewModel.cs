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
				return new Command(async() => await GetAllAthletes());
			}
		}

		public void LocalRefresh()
		{
			AllAthletes.Clear();
			DataManager.Instance.Athletes.Values.ToList().ForEach(AllAthletes.Add);
		}

		async public Task GetAllAthletes()
		{
			AllAthletes.Clear();
			using(new Busy(this))
			{
				await Task.Delay(1000);
				await AzureService.Instance.GetAllAthletes();
				LocalRefresh();
			}
		}
	}
}

