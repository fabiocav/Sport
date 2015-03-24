using System;
using System.Windows.Input;
using Xamarin.Forms;
using System.Threading.Tasks;

[assembly: Dependency(typeof(SportChallengeMatchRank.Shared.AthleteDetailsViewModel))]

namespace SportChallengeMatchRank.Shared
{
	public class AthleteDetailsViewModel : BaseViewModel
	{
		public AthleteDetailsViewModel()
		{
			Athlete = new Athlete();
		}

		public AthleteDetailsViewModel(Athlete athlete = null)
		{
			Athlete = athlete ?? new Athlete();
		}

		Athlete _athlete;
		public const string AthletePropertyName = "Athlete";

		public Athlete Athlete
		{
			get
			{
				return _athlete;
			}
			set
			{
				SetProperty(ref _athlete, value, AthletePropertyName);
			}
		}

		public ICommand SaveAthleteCommand
		{
			get
			{
				return new Command(async(param) =>
					await SaveAthlete());
			}
		}

		async public Task SaveAthlete()
		{
			using(new Busy(this))
			{
				try
				{
					await AzureService.Instance.SaveAthlete(Athlete);
				}
				catch(Exception e)
				{
					Console.WriteLine(e);
				}
			}
		}

		async public Task DeleteAthlete()
		{
			using(new Busy(this))
			{
				try
				{
					await AzureService.Instance.DeleteAthlete(Athlete.Id);
				}
				catch(Exception e)
				{
					Console.WriteLine(e);
				}
			}
		}
	}
}