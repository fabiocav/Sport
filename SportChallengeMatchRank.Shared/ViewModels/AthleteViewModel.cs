using System.Threading.Tasks;

namespace SportChallengeMatchRank.Shared
{
	public class AthleteViewModel : BaseViewModel
	{
		public AthleteViewModel(Athlete athlete)
		{
			Athlete = athlete;	
		}

		public Athlete Athlete
		{
			get;
			set;
		}

		async public Task GetLeagues(bool forceRefresh = false)
		{
			if(Athlete == null)
				return;

			if(!forceRefresh)
			{
				Athlete.RefreshMemberships();
				return;
			}

			if(IsBusy)
				return;

			using(new Busy(this))
			{
				Athlete.RefreshMemberships();

				var task = AzureService.Instance.GetAllLeaguesForAthlete(App.CurrentAthlete);
				await RunSafe(task);

				if(task.IsFaulted)
					return;

				Athlete.RefreshMemberships();
				SetPropertyChanged("Athlete");
			}

			IsBusy = false;
		}
	}
}

