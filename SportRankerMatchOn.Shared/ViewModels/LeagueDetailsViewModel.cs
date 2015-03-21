using System;
using System.Windows.Input;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace SportRankerMatchOn.Shared
{
	public class LeagueDetailsViewModel : BaseViewModel
	{
		League _league;
		public const string LeaguePropertyName = "League";

		public LeagueDetailsViewModel()
		{
			League = new League();
		}

		public LeagueDetailsViewModel(League league = null)
		{
			League = league ?? new League();
		}

		public League League
		{
			get
			{
				return _league;
			}
			set
			{
				SetProperty(ref _league, value, LeaguePropertyName);
			}
		}

		public ICommand SaveLeagueCommand
		{
			get
			{
				return new Command(async(param) =>
					{
						using(new Busy(this))
						{
							try
							{
								await AzureService.Instance.SaveLeague(League);
								await AzureService.Instance.AddAthleteToLeague(App.CurrentAthlete.Id, League.Id);
							}
							catch(Exception e)
							{
								Console.WriteLine(e);
							}
						}
					});
			}
		}
	}
}